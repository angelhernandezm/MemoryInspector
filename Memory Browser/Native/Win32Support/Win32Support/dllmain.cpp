// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 


/* File             : dllmain.cpp
   C++ compiler     : Microsoft (R) C/C++ Optimizing Compiler Version 16.00.40219.01 for x64
   Creation date    : 30/03/2011
   Developer        : Angel Hernández Matos
   e-m@il           : angel@bonafideideas.com 
				    : angeljesus14@hotmail.com
   Website          : http://www.bonafideideas.com
*/


#include "stdafx.h"
#include "dllmain.h"

BOOL APIENTRY DllMain( HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
	)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}


// Internal functions

//
// Method responsible for enabling privileges (debug) to a thread token
//
BOOL SetPrivilege(HANDLE hToken, LPCTSTR Privilege, BOOL bEnablePrivilege) {
	LUID luid; 
	BOOL retval = FALSE;
	TOKEN_PRIVILEGES tp = {0}; 
	DWORD cb=sizeof(TOKEN_PRIVILEGES); 

	if(LookupPrivilegeValue(NULL, Privilege, &luid )) {
		tp.PrivilegeCount = 1; 
		tp.Privileges[0].Luid = luid;
		tp.Privileges[0].Attributes = bEnablePrivilege ? SE_PRIVILEGE_ENABLED : 0;
		AdjustTokenPrivileges( hToken, FALSE, &tp, cb, NULL, NULL ); 

		if (GetLastError() == ERROR_SUCCESS)
			retval = TRUE;
	}
	return retval;
}

//
// Method responsible for obtaining  modules' information that belong to a process
//
BOOL GetModules(DWORD processId, ProcessInformation& process) {
	BOOL retval = FALSE;
	DWORD moduleCount = 0;
	MODULEINFO moduleDetails;
	wchar_t fullPath[MAX_PATH];
	HANDLE hProcess = NULL, hToken = NULL;
	HMODULE loadedModules[MAX_ITEM_COUNT];
	ZeroMemory(&loadedModules, sizeof(loadedModules));

	if ((hToken = GetThreadToken()) != NULL) {
		SetPrivilege(hToken, SE_DEBUG_NAME, TRUE); 
		if ((hProcess = OpenProcess(PROCESS_ALL_ACCESS, TRUE, processId)) != NULL) {
			if ((EnumProcessModulesEx(hProcess, loadedModules, sizeof(loadedModules), &moduleCount, LIST_MODULES_ALL)) != NULL) {

				// Is it x64 or x86? 
#ifdef _WIN64
				moduleCount = ITEM_COUNT(moduleCount) / 2; 
#else
				moduleCount = ITEM_COUNT(moduleCount); 
#endif

				// Base address corresponds to the first item returned by EnumProcessModulesEx
				process.entryPoint = (int) loadedModules[0]; 
				//// Get process' name
				GetProcessImageFileName(hProcess, fullPath, sizeof(fullPath));
				ExtractImageName(fullPath, process.processName); 
				for (int nModule = 0; nModule < (int) moduleCount; nModule++) {
					ModuleInformation currentModule;
					currentModule.hModule = loadedModules[nModule];
					if ((GetModuleInformation(hProcess, loadedModules[nModule], &moduleDetails, sizeof(moduleDetails))) != NULL) {
						currentModule.details = moduleDetails;
						GetModuleFileNameEx(hProcess, loadedModules[nModule], fullPath,  ARRAY_SIZE(fullPath));
						wcscpy_s(currentModule.modulePath, ARRAY_SIZE(fullPath), fullPath);
						ExtractImageName(fullPath, currentModule.moduleName);
						process.loaded.push_back(currentModule);
					}
				}
				retval = TRUE;
			}
			CloseHandle(hProcess);
		}
		SetPrivilege(hToken, SE_DEBUG_NAME, FALSE);
		CloseHandle(hToken);
	}
	return retval;
}


//
// Method responsible for obtaining processes and associated modules
//
DWORD GetProcessInformation(std::vector<ProcessInformation>& processes) {
	DWORD retval = 0, itemCount = 0;
	DWORD runningProcesses[MAX_ITEM_COUNT];
	ZeroMemory(&runningProcesses, sizeof(runningProcesses));

	if (EnumProcesses(runningProcesses, sizeof(runningProcesses), &itemCount)) {
		itemCount = ITEM_COUNT(itemCount);
		for (int nProcess = 0; nProcess < (int) itemCount; nProcess++) {
			ProcessInformation current;
			current.processId = runningProcesses[nProcess];
			if (GetModules(runningProcesses[nProcess], current))   {
				processes.push_back(current);
				retval++;
			}
		}
	}
	return retval;
}

//
// Method responsible for obtaining a Thread Token
//
HANDLE GetThreadToken() {
	HANDLE retval;
	int flag = TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY;

	if (!OpenThreadToken(GetCurrentThread(), flag, FALSE, &retval)) {
		if (GetLastError() == ERROR_NO_TOKEN)  {
			if (ImpersonateSelf(SecurityImpersonation) && 
				!OpenThreadToken(GetCurrentThread(), flag, FALSE, &retval)) 
				retval = NULL;
		}
	}
	return retval;
}

//
// Method responsible for extracting the file name from a full path
//
void ExtractImageName(const wchar_t* fullPath, wchar_t* imageName) {
	int pathLength = (int) wcslen(fullPath) - 1;
	int imageLength = (int) wcslen(wcsrchr(fullPath, '\\') + 1);
	int nPos = imageLength - 1;

	for (int nCount = pathLength; nPos >= 0 ; nCount--) {
		imageName[nPos] = fullPath[nCount];
		nPos--;
	}
	imageName[imageLength] = '\0';
}


//
// Method called via SymEnumSymbols when retrieving symbols 
//
BOOL CALLBACK SymEnumSymbolsProc(__in PSYMBOL_INFO pSymInfo, ULONG SymbolSize, __in_opt  PVOID UserContext) {
	size_t convertedChars;
	wchar_t symbolName[100];
	wchar_t buffer[MAX_BUFFER_SIZE];
	ptrSymbolInformation info = reinterpret_cast<ptrSymbolInformation>(UserContext);

	mbstowcs_s(&convertedChars, symbolName, pSymInfo->NameLen + 1, pSymInfo->Name, _TRUNCATE);

	StringCbPrintfW(buffer, BUFFER_SIZE(buffer), 
		L"<symbol name=\"%s\" id=\"%d\" AddressInHex=\"0x%X\" AddressInDec=\"%d\"/>",
		symbolName, info->symbolCount + 1, pSymInfo->Address, pSymInfo->Address);

	wcscat_s(*info->symbols, info->maxChars, buffer);

	info->symbolCount++;

	return TRUE;
}

HRESULT ReadMemoryHelper(const HANDLE hProcess, wchar_t** buffer, int maxChars, DWORD64 lpBaseAddress, int bytesToRead) {
	SIZE_T reads = 0;
	BOOL flag = TRUE;
	BYTE selectedByte;
	HRESULT retval = E_FAIL;
	DWORD64 offsetCount = 0;
	int lineNumber = 0, totalReads = 0;
	LPVOID data, startAddress = (LPVOID) lpBaseAddress;
	wchar_t hexBuffer[1024], decBuffer[1024], dumpSection[1024], tempBuffer[10];

	if (bytesToRead > 0 &&  lpBaseAddress > 0 && (data = VirtualAlloc(NULL, bytesToRead, MEM_COMMIT, PAGE_READWRITE)) != NULL) {
		offsetCount = (DWORD64) startAddress;
		while(flag) {
			flag = ReadProcessMemory(hProcess, startAddress, data, MAX_BLOCK_SIZE, &reads)	&&
									((totalReads += reads) < bytesToRead);

			for (int nByte = 0; nByte < (int)reads; nByte += BYTES_PER_LINE) {
				decBuffer[0]='\0';
				hexBuffer[0]='\0';
				tempBuffer[0]='\0';
				dumpSection[0]='\0';

				for (int offset = 0; offset < BYTES_PER_LINE; offset++) {
					selectedByte = *(((LPBYTE) data) + nByte + offset);
					StringCbPrintfW(tempBuffer, BUFFER_SIZE(tempBuffer), L"%02X ", selectedByte);

					wcscat_s(hexBuffer, BUFFER_SIZE(hexBuffer), tempBuffer);

					StringCbPrintfW(tempBuffer, BUFFER_SIZE(tempBuffer), L"%c ",
						(selectedByte >= NO_PRINTABLE_CHAR1 && selectedByte < NO_PRINTABLE_CHAR2 ? 
						(wchar_t)  selectedByte : (wchar_t) '.'));

					wcscat_s(decBuffer, BUFFER_SIZE(decBuffer), tempBuffer);
				}

				lineNumber++;

				StringCbPrintfW(dumpSection, BUFFER_SIZE(dumpSection),
					L"<dumpBlock lineNumber=\"%d\" address=\"0x%X\" contentAsHex=\"%s\"> <contentAsAscii><![CDATA[%s]]></contentAsAscii></dumpBlock>",
					lineNumber, offsetCount , hexBuffer, decBuffer); 

					offsetCount += BYTES_PER_LINE;
					wcscat_s(*buffer, maxChars, dumpSection); 
				}
				startAddress = (LPVOID) (((DWORD64) startAddress) + ((DWORD64) reads));
			}
			retval = S_OK;
			VirtualFree(data, 0, MEM_RELEASE);
	}
	return retval;
}

//
// Helper method to Execute
//
HRESULT ExecuteHelper(const HANDLE hProcess, wchar_t** buffer, int maxChars, Operation oper, const ProcessInformation* pInfo, __in_opt const wchar_t* libraryName) {
	char* module;
	HRESULT retval = S_OK;
	DWORD64 baseAddress = 0;
	HMODULE hModule = NULL; 
	MODULEINFO moduleInfo;
	ptrSymbolInformation info;
	MEMORY_BASIC_INFORMATION mbi;
	wchar_t tempBuffer[MAX_BUFFER_SIZE];
	IMAGEHLP_MODULE64 im = { sizeof(im) };
	int cbCount = 0, result = 0, pathLen = 0;
	PSYM_ENUMERATESYMBOLS_CALLBACK enumCallback;

	switch(oper) {
	case GetImageInformation:
		wcscat_s(*buffer, maxChars, L"<?xml version=\"1.0\" encoding=\"utf-8\" ?><memoryMap>");

		StringCbPrintfW(tempBuffer, BUFFER_SIZE(tempBuffer), L"<process name=\"%s\" id=\"%d\" modulesCount=\"%d\" baseAddressInHex=\"0x%X\" baseAddressInDec=\"%d\"/><modules>",
			pInfo->processName, pInfo->processId, pInfo->loaded.capacity(), pInfo->entryPoint, pInfo->entryPoint);

		wcscat_s(*buffer, maxChars, tempBuffer);

		std::for_each(pInfo->loaded.begin(), pInfo->loaded.end(), [&] (ModuleInformation current) {
			StringCbPrintfW(tempBuffer, BUFFER_SIZE(tempBuffer), L"<module id=\"%d\" name=\"%s\" entryPointInHex=\"0x%X\" entryPointInDec=\"%d\" baseOfDllInHex=\"0x%X\" baseOfDllInDec=\"%d\" sizeOfImage=\"%d\" path=\"%s\"/>",
				++cbCount, current.moduleName, current.details.EntryPoint, current.details.EntryPoint, current.details.lpBaseOfDll, 
				current.details.lpBaseOfDll, current.details.SizeOfImage, current.modulePath);

			wcscat_s(*buffer, maxChars, tempBuffer);
		});
		wcscat_s(*buffer, maxChars, L"</modules></memoryMap>");
		break;

	case GetSymbols:
		pathLen = (int) wcslen(libraryName);
		module = new char[pathLen + 1];
		wcscat_s(*buffer, maxChars, L"<?xml version=\"1.0\" encoding=\"utf-8\" ?><symbols><exports>");
		info = new SymbolInformation;
		info->symbols = buffer;
		info->maxChars = maxChars;
		info->symbolCount = 0;
		enumCallback = SymEnumSymbolsProc;
		SymSetOptions(SymGetOptions() | SYMOPT_DEBUG);
		result = WideCharToMultiByte(CP_UTF8, NULL, libraryName, pathLen, module, pathLen , NULL, NULL);

		if (result == sizeof(module))
			module[result - 1] = NULL;
		else module[result] = NULL; 

		if (SymInitialize(hProcess, NULL, TRUE)) {
			baseAddress = SymLoadModuleEx(hProcess, NULL, module, NULL, 0, 0, NULL, 0);
			if (SymGetModuleInfo64(hProcess, baseAddress, &im)) {
				SymEnumSymbols(hProcess, im.BaseOfImage, NULL, enumCallback, info);
				wcscat_s(*buffer, maxChars, L"</exports></symbols>");
				SymUnloadModule64(hProcess, im.BaseOfImage);
				SymCleanup(hProcess);
			}
		} else retval = E_FAIL;
		delete[] module;
		delete info;
		break;

	case GetAllocations:
		memset(&moduleInfo, 0, sizeof(MODULEINFO));
		memset(&mbi, 0, sizeof(MEMORY_BASIC_INFORMATION));
		auto module = std::find_if(pInfo->loaded.begin(), pInfo->loaded.end(), FindStringMatch(libraryName));

		if (module != pInfo->loaded.end()) {
			wcscat_s(*buffer, maxChars, L"<?xml version=\"1.0\" encoding=\"utf-8\" ?><allocations>");
			GetAllocationsHelper(hProcess, reinterpret_cast<ModuleInformation*>(module._Ptr), buffer, maxChars);
			wcscat_s(*buffer, maxChars, L"</allocations>");
		} else retval = E_FAIL;
		break;
	}

	return retval;
}

// Exported functions

//
// Method responsible for analizing processes, modules and allocations
//
EXPORT HRESULT Execute(const wchar_t* imageName, wchar_t* infoAsXml, int maxChars, Operation oper,  __in_opt const wchar_t* libraryName) {
	HRESULT retval = E_FAIL;
	HANDLE hProcess = NULL, hToken = NULL;
	wchar_t* buffer = new wchar_t[maxChars];
	std::vector<ProcessInformation> processes;
	DWORD processCount = GetProcessInformation(processes);
	auto found = std::find_if(processes.begin(), processes.end(), FindByNameOrId(imageName));

	if (found != processes.end()) {
		if (SetPrivilege((hToken = GetThreadToken()), SE_DEBUG_NAME, TRUE) &&
			(hProcess = OpenProcess(PROCESS_ALL_ACCESS, TRUE, found->processId)) != NULL) {
				buffer[0] = '\0';
				retval = ExecuteHelper(hProcess, &buffer, maxChars, oper, reinterpret_cast<ProcessInformation*>(found._Ptr), libraryName);
				CloseHandle(hProcess);
				SetPrivilege(hToken, SE_DEBUG_NAME, FALSE);
				CloseHandle(hToken);
		} 
	}
	StringCchCopyW(infoAsXml, maxChars, buffer);
	delete[] buffer;
	return retval;
}

EXPORT HRESULT ReadMemory(const wchar_t* imageName, wchar_t* infoAsXml, int maxChars, DWORD64 lpBaseAddress, int bytesToRead) {
	HRESULT retval = E_FAIL;
	HANDLE hProcess = NULL, hToken = NULL;
	wchar_t* buffer = new wchar_t[maxChars];
	std::vector<ProcessInformation> processes;
	DWORD processCount = GetProcessInformation(processes);
	auto found = std::find_if(processes.begin(), processes.end(), FindByNameOrId(imageName));

	if (found != processes.end()) {
		if (SetPrivilege((hToken = GetThreadToken()), SE_DEBUG_NAME, TRUE) &&
			(hProcess = OpenProcess(PROCESS_ALL_ACCESS, TRUE, found->processId)) != NULL) {
				buffer[0] = '\0';
				wcscat_s(buffer, maxChars, L"<?xml version=\"1.0\" encoding=\"utf-8\" ?><memoryDump>");
				retval = ReadMemoryHelper(hProcess, &buffer, maxChars, lpBaseAddress, bytesToRead);
				wcscat_s(buffer, maxChars, L"</memoryDump>");
				CloseHandle(hProcess);
				SetPrivilege(hToken, SE_DEBUG_NAME, FALSE);
				CloseHandle(hToken);
		} 
	}
	StringCchCopyW(infoAsXml, maxChars, buffer);
	delete[] buffer;
	return retval;
}

//
// Method responsible for walking through the pages of a specific module
//
void GetAllocationsHelper(const HANDLE hProcess, const ModuleInformation* module, wchar_t** buffer, int maxChars) {
	int allocationCount = 0;
	MEMORY_BASIC_INFORMATION mbi;
	wchar_t tempBuffer[MAX_BUFFER_SIZE];
	LPVOID lpAddress = module->details.lpBaseOfDll;
	memset(&mbi, 0, sizeof(MEMORY_BASIC_INFORMATION));

	do {
		StringCbPrintfW(tempBuffer, BUFFER_SIZE(tempBuffer), L"<allocation id=\"%d\" baseAddressInDec=\"%d\" baseAddressInHex=\"0x%X\" allocationBaseInHex=\"0x%X\" allocationProtect=\"%s\" regionSize=\"%ld\" state=\"%s\" type=\"%s\" />",
			allocationCount + 1, mbi.BaseAddress, mbi.BaseAddress, mbi.AllocationBase, GetTranslationForMemoryValue(mbi.AllocationProtect, MemoryTranslation::Protection),
			mbi.RegionSize,  GetTranslationForMemoryValue(mbi.State, MemoryTranslation::State),
			GetTranslationForMemoryValue(mbi.Type, MemoryTranslation::Type));

		wcscat_s(*buffer, maxChars,tempBuffer);
		lpAddress = (LPVOID)((DWORD64)mbi.BaseAddress + (DWORD64)mbi.RegionSize);
		allocationCount++;
	} while (VirtualQueryEx(hProcess, lpAddress, &mbi, sizeof(mbi)) > 0); 
}

//
// Method responsible for translating the page protection & state into a string
//
const wchar_t* GetTranslationForMemoryValue(DWORD dwMemVal, MemoryTranslation translation) {
	wchar_t* retval = L"";

	switch(translation) {
	case MemoryTranslation::Protection:
		if (dwMemVal & PAGE_READONLY) 
			retval = L"ReadOnly";

		if (dwMemVal & PAGE_READWRITE)  
			retval = L"ReadWrite";

		if (dwMemVal & PAGE_WRITECOPY)    
			retval = L"WriteCopy";

		if (dwMemVal & PAGE_EXECUTE)    
			retval = L"Execute";

		if (dwMemVal & PAGE_EXECUTE_READ)   
			retval = L"ExecuteRead";

		if (dwMemVal & PAGE_EXECUTE_READWRITE)   
			retval = L"ExecuteReadWrite";

		if (dwMemVal & PAGE_EXECUTE_WRITECOPY)
			retval = L"ExecuteWriteCopy";

		if (dwMemVal & PAGE_GUARD) 
			retval = L"Guard";

		if (dwMemVal & PAGE_NOACCESS) 
			retval = L"NoAccess";

		if (dwMemVal & PAGE_NOCACHE)  
			retval = L"NoCache";

		break;
	case MemoryTranslation::State:
		switch(dwMemVal) {
		case MEM_COMMIT:
			retval = L"Commit";
			break;
		case MEM_FREE:
			retval = L"Free";
			break;
		case MEM_RESERVE:
			retval = L"Reserved";
			break;
		default:
			retval = L"Unknown";
			break;
		}
		break;

	case MemoryTranslation::Type:
		switch(dwMemVal) {
		case MEM_IMAGE:
			retval = L"Image";
			break;
		case MEM_MAPPED:
			retval = L"Mapped";
			break;
		case MEM_PRIVATE:
			retval = L"Private";
			break;
		}
		break;
	}
	return retval;
}
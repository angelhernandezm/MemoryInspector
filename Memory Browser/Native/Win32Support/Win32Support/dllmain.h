// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 


/* File             : dllmain.h
   C++ compiler     : Microsoft (R) C/C++ Optimizing Compiler Version 16.00.40219.01 for x64
   Creation date    : 30/03/2011
   Developer        : Angel Hernández Matos
   e-m@il           : angel@bonafideideas.com 
					: angeljesus14@hotmail.com
   Website          : http://www.bonafideideas.com
*/


#include "stdafx.h"

// Exported functions
EXPORT HRESULT ReadMemory(const wchar_t* imageName, wchar_t* infoAsXml, int maxChars, DWORD64 lpBaseAddress, int bytesToRead);
EXPORT HRESULT Execute(const wchar_t* imageName, wchar_t* infoAsXml, int maxChars,  Operation oper,  __in_opt const wchar_t* libraryName);

// Internal functions
HANDLE GetThreadToken();
BOOL GetModules(DWORD processId, ProcessInformation& modules);
void ExtractImageName(const wchar_t* fullPath, wchar_t* imageName);
DWORD GetProcessInformation(std::vector<ProcessInformation>& processes);
BOOL SetPrivilege(HANDLE hToken, LPCTSTR Privilege, BOOL bEnablePrivilege);
const wchar_t* GetTranslationForMemoryValue(DWORD dwMemVal, MemoryTranslation translation);
BOOL CALLBACK SymEnumSymbolsProc(__in PSYMBOL_INFO pSymInfo, ULONG SymbolSize, __in_opt  PVOID UserContext);
void GetAllocationsHelper(const HANDLE hProcess, const ModuleInformation* module, wchar_t** buffer, int maxChars);
HRESULT ReadMemoryHelper(const HANDLE hProcess, wchar_t** buffer, int maxChars, DWORD64 lpBaseAddress, int bytesToRead);
HRESULT ExecuteHelper(const HANDLE hProcess, wchar_t** buffer, int maxChars, Operation oper, const ProcessInformation* pInfo, __in_opt const wchar_t* libraryName);
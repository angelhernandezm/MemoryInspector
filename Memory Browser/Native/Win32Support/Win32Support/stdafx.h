// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 


/* File             : stdafx.h
   C++ compiler     : Microsoft (R) C/C++ Optimizing Compiler Version 16.00.40219.01 for x64
   Creation date    : 30/03/2011
   Developer        : Angel Hernández Matos
   e-m@il           : angel@bonafideideas.com 
				    : angeljesus14@hotmail.com
   Website          : http://www.bonafideideas.com
*/


// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>

// TODO: reference additional headers your program requires here
#include <stdio.h>
#include <algorithm>
#include <iostream>
#include <tchar.h>
#include <stdlib.h>
#include <vector>
#include <conio.h>
#include <Psapi.h>
#include <string.h>
#include <wchar.h>
#include <Winternl.h>
#include <Tlhelp32.h>
#include <Dbghelp.h>
#include <strsafe.h>

//
// Macros
//
#define EXPORT extern "C" __declspec(dllexport)
#define MAX_ITEM_COUNT 0x0000400
#define ARRAY_SIZE(x) (sizeof(x)/sizeof(x[0]))
#define ITEM_COUNT(x) ( x > 0 ? (x/sizeof(x)) : 0)
#define MAX_BLOCK_SIZE 0x000400              // 1024 bytes to be read at a time  
#define BYTES_PER_LINE 0x000010              // Bytes to be read/displayed by line
#define NO_PRINTABLE_CHAR1  0x000020
#define NO_PRINTABLE_CHAR2  0x00007F
#define MAX_BUFFER_SIZE     0x000200
#define BUFFER_SIZE(x) (MAX_BUFFER_SIZE * sizeof(wchar_t))

//
// Structs
//
typedef struct _ModInfo {
	HMODULE hModule;
	MODULEINFO details;
	wchar_t moduleName[MAX_PATH];
	wchar_t modulePath[MAX_PATH];
}*ptrModuleInformation, ModuleInformation; 

typedef struct _ProcessInformation {
	DWORD processId;
	int entryPoint;
	wchar_t processName[100];
	std::vector<ModuleInformation> loaded;
} *ptrProcessInformation, ProcessInformation;

typedef struct _SymbolInformation {
	int maxChars;
	int symbolCount;
	wchar_t** symbols; 
}*ptrSymbolInformation, SymbolInformation;

//
// Enums
//
typedef enum _MemoryTranslation {
	State = 0,
	Protection,
	Type
} MemoryTranslation;

typedef enum _Operation {
	GetImageInformation = 0,
	GetSymbols,
	GetAllocations
} Operation;

//
// Functors
//
typedef struct FindByNameOrId: std::unary_function<ProcessInformation, bool> {
	wchar_t image[MAX_PATH];
	FindByNameOrId(const wchar_t* _image) {
		wcscpy_s(image, ARRAY_SIZE(image), _image);
	} 

	bool operator()(const ProcessInformation& selected) {
		return (_wcsnicmp(selected.processName, image, ARRAY_SIZE(selected.processName)) == 0 ||
			_wtoi(image) == selected.processId);
	} 
};

typedef struct FindStringMatch: std::unary_function<ModuleInformation, bool> {
	wchar_t image[MAX_PATH];
	FindStringMatch(const wchar_t* _image) {
		wcscpy_s(image, ARRAY_SIZE(image), _image);
	} 

	bool operator()(const ModuleInformation& selected) {
		return (_wcsnicmp(selected.moduleName, image, ARRAY_SIZE(selected.moduleName)) == 0);
	} 
};
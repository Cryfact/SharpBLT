#define WIN32_LEAN_AND_MEAN 1
#include <windows.h>

#include <memory>

#pragma pack(1)

#pragma region ALLFUNC(FUNC)

// Export with https://www.nirsoft.net/utils/dll_export_viewer.html into wsock.def.pre
// Then run
//   awk 'BEGIN {print "#define ALLFUNC(FUNC) \\"} { print "\tFUNC(" i++ "," $1 "," $4 ") \\" } \
//   END {print ""; print "#define ALLFUNC_COUNT " NR}' wsock.def.pre
// Or to generate the definition file, use
//   awk 'BEGIN { print "EXPORTS" } { print $1 "=_WSOCK_EXPORT_" $1 " @" $4 } ' wsock.def.pre > wsock.def

#define ALLFUNC(FUNC)                 \
	FUNC(__WSAFDIsSet)                \
	FUNC(accept)                      \
	FUNC(AcceptEx)                    \
	FUNC(bind)                        \
	FUNC(closesocket)                 \
	FUNC(connect)                     \
	FUNC(dn_expand)                   \
	FUNC(EnumProtocolsA)              \
	FUNC(EnumProtocolsW)              \
	FUNC(GetAcceptExSockaddrs)        \
	FUNC(GetAddressByNameA)           \
	FUNC(GetAddressByNameW)           \
	FUNC(gethostbyaddr)               \
	FUNC(gethostbyname)               \
	FUNC(gethostname)                 \
	FUNC(GetNameByTypeA)              \
	FUNC(GetNameByTypeW)              \
	FUNC(getnetbyname)                \
	FUNC(getpeername)                 \
	FUNC(getprotobyname)              \
	FUNC(getprotobynumber)            \
	FUNC(getservbyname)               \
	FUNC(getservbyport)               \
	FUNC(GetServiceA)                 \
	FUNC(GetServiceW)                 \
	FUNC(getsockname)                 \
	FUNC(getsockopt)                  \
	FUNC(GetTypeByNameA)              \
	FUNC(GetTypeByNameW)              \
	FUNC(htonl)                       \
	FUNC(htons)                       \
	FUNC(inet_addr)                   \
	FUNC(inet_network)                \
	FUNC(inet_ntoa)                   \
	FUNC(ioctlsocket)                 \
	FUNC(listen)                      \
	FUNC(MigrateWinsockConfiguration) \
	FUNC(NPLoadNameSpaces)            \
	FUNC(ntohl)                       \
	FUNC(ntohs)                       \
	FUNC(rcmd)                        \
	FUNC(recv)                        \
	FUNC(recvfrom)                    \
	FUNC(rexec)                       \
	FUNC(rresvport)                   \
	FUNC(s_perror)                    \
	FUNC(select)                      \
	FUNC(send)                        \
	FUNC(sendto)                      \
	FUNC(sethostname)                 \
	FUNC(SetServiceA)                 \
	FUNC(SetServiceW)                 \
	FUNC(setsockopt)                  \
	FUNC(shutdown)                    \
	FUNC(socket)                      \
	FUNC(TransmitFile)                \
	FUNC(WEP)                         \
	FUNC(WSAAsyncGetHostByAddr)       \
	FUNC(WSAAsyncGetHostByName)       \
	FUNC(WSAAsyncGetProtoByName)      \
	FUNC(WSAAsyncGetProtoByNumber)    \
	FUNC(WSAAsyncGetServByName)       \
	FUNC(WSAAsyncGetServByPort)       \
	FUNC(WSAAsyncSelect)              \
	FUNC(WSACancelAsyncRequest)       \
	FUNC(WSACancelBlockingCall)       \
	FUNC(WSACleanup)                  \
	FUNC(WSAGetLastError)             \
	FUNC(WSAIsBlocking)               \
	FUNC(WSApSetPostRoutine)          \
	FUNC(WSARecvEx)                   \
	FUNC(WSASetBlockingHook)          \
	FUNC(WSASetLastError)             \
	FUNC(WSAStartup)                  \
	FUNC(WSAUnhookBlockingHook)       \

#pragma endregion

// For some reason PhysX explodes if you store these in an array on 64-bit! I have no idea why, but it's fine if we use
// a struct, so...
struct farproc_dll
{
#define STRUCT(name) FARPROC o##name;
	ALLFUNC(STRUCT)
#undef STRUCT
} farproc;

bool Init_wsock()
{
	char bufd[200];
	GetSystemDirectoryA(bufd, 200);
	strcat_s(bufd, "\\WSOCK32.dll");

	HMODULE hL = LoadLibraryA(bufd);
	if (!hL) return false;

#define REGISTER(name) farproc.o##name = GetProcAddress(hL, #name);
	ALLFUNC(REGISTER);
#undef REGISTER

	return true;
}

extern "C"
{
	FARPROC PA = 0;

#if defined(_M_AMD64)
	int jumpToPA();

#define DEF_STUB(name) \
	void _WSOCK_EXPORT_##name() \
	{ \
		PA = farproc.o##name; \
		jumpToPA(); \
	}; \

	ALLFUNC(DEF_STUB)
#undef DEF_STUB

#else
#define DEF_STUB(name) \
	extern "C" __declspec(naked) void __stdcall _WSOCK_EXPORT_##name() { \
		__asm { \
			jmp farproc.o##name \
		} \
	}; \

	ALLFUNC(DEF_STUB)
#undef DEF_STUB

#endif
}
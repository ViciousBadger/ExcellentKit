// This magic code makes C# 9 init-only setters compile.
// See https://docs.unity3d.com/6000.0/Documentation/Manual/csharp-compiler.html
namespace System.Runtime.CompilerServices
{
    class IsExternalInit { }
}

# Dynamic Dll

## Instruction

* Load extra C# code on the fly.
* Depends on [Lightning Rebuild](https://github.com/bluesky139/UnityCommonUtilities/blob/master/docs/LightningRebuild.md).

## Limit

* For Android mono2x build only.
* Can't replace whole class or method, but you can directly call all methods, also you can use reflection to do more.

## Usage

### Before release apk
* Prepare a place where the dll will be put, download it to local.
* Call `DynamicDllLoader.SetDll()` to decode this dll (code won't be run).
* Call `DynamicDllLoader.Load()` in somewhere to run code. (explain later)
* Call `DynamicDllLoader.UnloadAll()` in somewhere if necessary, you can use this to unload loaded dll, then load a new one.

### After release apk
* Create a new class, inherit from `MonoBehaviour`, Add `DynamicDll` attribute to this class, and provide a name, this name is used for `DynamicDllLoader.Load()`.
* Do whatever in this class.
* Make sure only one class in this file, class name need same as file name.
* Prepare `_LightningBuild` folder and apk, Use menu `Common -> Dynamic Dll -> Build` to build dll, output is `./_LightningRebuild/Assembly-CSharp-dynamic.dll`.
* Put it to `/sdcard/Android/data/[PackageName]/files/` to test (I will load it from here for local test), make sure everything is ok then put it online.
* Demo: [https://github.com/bluesky139/UnityCommonUtilities/tree/master/Assets/Test/DynamicDll](https://github.com/bluesky139/UnityCommonUtilities/tree/master/Assets/Test/DynamicDll)
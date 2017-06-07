# Lightning Rebuild

## Reason

* Time cost too much to build whole project, especially on big project.
* Some bug can only be reproduced on mobile device, I have to build again and again even I only want to print more logs.
* I want to rebuild C# code only.

## How

* Unity will generate response file to build C# code, save these files for later rebuild.

## Limit

* For Android mono2x build only.
* Only works with modification of cs files.
* For debug only due to safety concerns.

## Usage

* Call `LightningRebuild.specCSharpFiles` to set specific C# file paths if necessary, such as generated C# files, these files will be backed up for later compilation. Each file should starts with `Assets/`.
* Call `LightningRebuild.PreBuild()` before `BuildPipeline.BuildPlayer()`.
* Make sure no lightning rebuild related error after build.
* The generated folder `./_LightningRebuild` is for lightning rebuild, if it's built on other machine, you can copy it to your machine.
* Copy built apk to `./`.
* Use menu `Common -> LightningRebuild -> Rebuild xxx` to rebuild, new output apk is `./_LightningRebuild/rebuilt.apk`.
* Demo: [https://github.com/bluesky139/UnityCommonUtilities/blob/master/Assets/Test/LightningRebuild/Editor/LightningRebuildEditorTest.cs](https://github.com/bluesky139/UnityCommonUtilities/blob/master/Assets/Test/LightningRebuild/Editor/LightningRebuildEditorTest.cs)
# Floating Debug

## Introduction

* Any time any where to call some method to test.

## Usage

* Write static method in somewhere and do something in it as you want, add `FloatingDebug.Item()` attribute to this static method.
* Call `FloatingDebug.Create()` at game start, it will create a test entrance for you.
* Run game, tap entrance button to list methods which you just add in 1st step, find and run your test method.
* Demo: [https://github.com/bluesky139/UnityCommonUtilities/blob/master/Assets/Test/FloatingDebug/FloatingDebugTest.cs](https://github.com/bluesky139/UnityCommonUtilities/blob/master/Assets/Test/FloatingDebug/FloatingDebugTest.cs)
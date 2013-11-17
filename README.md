Ben's LPT [![Build Status](https://travis-ci.org/benjojo/LPT.png?branch=master)](https://travis-ci.org/benjojo/LPT)
=============
Licence Plate Tagger

## Please be aware this code is pretty horrific.
This code was mainly a experiment, Because of a lack of research I probably did it *completely* wrong

However, the code here does its job (most of the time) and will successfully tag UK Yellow Licence plates.  


##Branches

This repo is moved into branches because of the way I made the program back in September 2012

###Single Plate
![Single Plate](/Docs/Rev1.PNG)

This was the first version that was more of a POC, it can track a single LP and just about work.

Later on I had discovered that I had done a transformation incorrectly removing the warping on the plates

![S2 Plate](/Docs/Rev2.png)

###Multiplate
![MPlate](/Docs/multiplate.PNG)

This version can track more then one plate at a time with the downside of having the ability of jumping around like a crazy mad man. Mostly the most usable version however.

##Issues

Because of the way I had visioned this in my mind in September. This plate detection system only works on the **back** of cars with **yellow** plates.

Because the algro is deeply attached to the bright yellow color, it may pick out double yellow lines of other stupid stuff like shown below.

![YellowIssue](/Docs/yellowvan.PNG)

##Usage

This program takes a large stash of images and will output a final image.


####Licence
[GPL](http://opensource.org/licenses/gpl-2.0.php)

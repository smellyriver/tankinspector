# :exclamation::exclamation: CONTRIBUTORS NEEDED :exclamation::exclamation:
As two once-upon-a-time serious World of Tanks players, Hillin and Edward has created this project in 2013 and kept it maintained ever since. However, we are not playing World of Tanks for years, and the game has evolved quite a lot since our absence. Hence we are not able to catch up with the most recent changes, nor Tank Inspector. Besides, we barely have time to put our hands on this project, so the fix of crashes, which is very likely to be introduced by every tiny game file change, tends to come late. 
We are here to ask for help. If you are a WoT player with adequate knowledge of C# programming (and some XAML when UI issues are involved), you can help by looking into these aspects:
- **Fix crashes introduced by new game versions**. These crashes will most likely to be caused by XML file changes. Actually we have a quite decent passive XML handler in [Tank Inspector PRO](https://github.com/smellyriver/tank-inspector-pro), which could avoid most of these crashes. Hopefully one day we can port it into Tank Inspector.
- **Implement new game mechanisms**. New mechanisms and change of old ones are constantly introduced by game updates. Tank Inspector is falling behind quite a lot, we need to catch up. This includes the new camouflage mechanism, battle boosters introduced in 9.19 etc.



# Tank Inspector
Tank Inspector is a beloved utility program of World of Tank players, in which you can inspect all the secrets of every tank in the game - model, armor scheme, specs and everything. First released in November 2013, this program acted as a great counsellor to millions of players.
And now it's open source.

## Get Tank Inspector
Go to the [Release Page](https://github.com/smellyriver/tankinspector/releases) to download the latest release.

## Build
This repository contains the source code of Tank Inspector. Open the solution file with Visual Studio 2017, restore packages from nuget, then you can compiled the project.

## Contribute
You can help us to make Tank Inspector better:
- [Fire an issue](https://github.com/smellyriver/tankinspector/issues) if you've found bugs while using Tank Inspector. Before you do that, please **first search for it, don't create duplicated issues**, and be sure to **post the log file** (*sti.log* under the installation folder).
- [Fork](https://github.com/smellyriver/tankinspector/new/master?readme=1#fork-destination-box) the project and improve it. Then you can send pull requests back to contribute to this project.
- [Donate](https://www.paypal.com/webapps/shoppingcart?flowlogging_id=4b7de2e4d8256&mfid=1494580630152_4b7de2e4d8256#/checkout/openButton) to us if you find this program useful. Donation took a really important part keeping Tank Inspector alive in the past years.

### About Tank Inspector PRO
This repository is for the original Tank Inspector. As a derived (yet lack of maintainance) work, Tank Inspector PRO is also open source. You can access it [here](https://github.com/smellyriver/tank-inspector-pro).

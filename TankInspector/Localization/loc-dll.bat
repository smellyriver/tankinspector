copy /y locbaml.exe %1\locbaml.exe
copy /y zh-hans.csv %1\zh-hans.csv
copy /y zh-hant.csv %1\zh-hant.csv
copy /y zh-tw.csv %1\zh-tw.csv
copy /y en-gb.csv %1\en-gb.csv
copy /y ko.csv %1\ko.csv
copy /y ru.csv %1\ru.csv
copy /y fr.csv %1\fr.csv
copy /y de.csv %1\de.csv
copy /y pl.csv %1\pl.csv
copy /y hu.csv %1\hu.csv
cd %1
md zh-HANS
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:zh-hans.csv /out:"zh-HANS" /cul:zh-HANS
md zh-TW
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:zh-tw.csv /out:"zh-TW" /cul:zh-TW
md zh-HANT
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:zh-hant.csv /out:"zh-HANT" /cul:zh-HANT
md en-GB
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:en-gb.csv /out:"en-GB" /cul:en-GB
md ko
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:ko.csv /out:"ko" /cul:ko
md ru
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:ru.csv /out:"ru" /cul:ru
md fr
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:fr.csv /out:"fr" /cul:fr
md de
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:de.csv /out:"de" /cul:de
md pl
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:pl.csv /out:"pl" /cul:pl
md hu
locbaml /generate en-US/Smellyriver.TankInspector.resources.dll /trans:hu.csv /out:"hu" /cul:hu
REM rd "en-US" /s /q
pause

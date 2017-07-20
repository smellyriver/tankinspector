copy /y %1\locbaml.exe %2\locbaml.exe
copy /y %1\zh-hans.csv %2\zh-hans.csv
copy /y %1\zh-hant.csv %2\zh-hant.csv
copy /y %1\zh-tw.csv %2\zh-tw.csv
copy /y %1\en-gb.csv %2\en-gb.csv
copy /y %1\ko.csv %2\ko.csv
copy /y %1\ru.csv %2\ru.csv
copy /y %1\fr.csv %2\fr.csv
copy /y %1\de.csv %2\de.csv
copy /y %1\pl.csv %2\pl.csv
copy /y %1\hu.csv %2\hu.csv
cd %2
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

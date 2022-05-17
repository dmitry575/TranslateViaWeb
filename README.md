# TranslateViaWeb
It is example how to use Selenium for translate many files.
For transalting using follow web services:

* https://www.bing.com
* https://www.deepl.com
* https://www.m-translate.by
* https://www.m-translate.ru
* https://translate.systran.net
* https://www.translate.ru
* https://www.translator.eu
* https://www.reverso.net


`TranslateViaWeb` is console application using follow parameters:

`/from` - from language translate, for example: en

`/to` - to language translate, for example: en

`/dir` - full directories from witch get all files and try to translate

`/output` - full directories where will be saving new files

`/timeout` - how many seconds wait loading page or another elemnts

The application using 1 thread for translate.

### Run

Tranlating all filed in path `Data` from `English` to `Russian` and result of translate put to the path `out`

```cmd
TranslateViaWeb.exe /from en /to ru /dir ./Data /output ./out
```

### Add resource for translating


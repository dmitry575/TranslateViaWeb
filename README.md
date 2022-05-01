# TranslateViaWeb
It is example how to use Selenium for translate list of files.
For transalting using different web services:
* https://www.bing.com
* https://www.deepl.com
* https://www.m-translate.by
* https://www.m-translate.ru
* https://translate.systran.net
* https://www.translate.ru
* https://www.translator.eu


TranslateViaWeb is console programm with parameters:

/from - from language translate, for example: en

/to - to language translate, for example: en

/dir - full directories from witch get all files and try to translate

/output - full directories where will be saving new files

/timeout - how many seconds wait loading page or another elemnts

The programm using 1 thread for translate.

#Example

`/from en /to ru /dir ./Data /output ./out`
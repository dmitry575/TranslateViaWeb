using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using TranslateViaWeb.Configs;
using TranslateViaWeb.Translates.Impl;

namespace TranslateViaWeb.Translates
{
    /// <summary>
    /// Handler of queue with files
    /// 
    //https://www.bing.com/translator
    //https://www.reverso.net/text_translation.aspx?lang=RU
    //https://www.translate.ru/
    //https://www.webtran.ru/
    //https://www.online-translator.com/
    //https://translation2.paralink.com/
    //https://www.translator.eu/
    //https://www.translate.com/
    //https://www.lexicool.com/translate.asp
    //https://www.systransoft.com/lp/free-online-translation/

    /// </summary>
    public class TranslateBackgroundHandler
    {
        private readonly Configuration _config;
        private readonly List<string> _files;
        private readonly ILog _logger = LogManager.GetLogger(typeof(TranslateBackgroundHandler));
        private readonly Type[] _translateFiles =
        {
            typeof(DeeplTranslateFile),
            typeof(MTranslateByTranslateFile),
            typeof(TranslateRuFile),
            typeof(BingFile),
        };
        private readonly CancellationToken _cancellationToken;

        public TranslateBackgroundHandler(Configuration config, List<string> files, CancellationToken cancellationToken)
        {
            _config = config;
            _cancellationToken = cancellationToken;
            _files = files;
        }

        /// <summary>
        /// Start Translate files in Task
        /// </summary>
        public void Work()
        {
            try
            {
                _logger.Info($"start translate {_files.Count} files");

                Task[] tasks = new Task[_translateFiles.Length];
                Dictionary<int, int> statistics = InitStatistics();
                int countRun = 0;
                var idx = -1;
                foreach (var file in _files)
                {
                    if (countRun >= _translateFiles.Length)
                    {
                        idx = Task.WaitAny(tasks, _cancellationToken);
                        tasks[idx] = null;
                        countRun--;
                        statistics[idx]++;
                    }
                    else
                    {
                        idx++;
                    }

                    var translate = TranslateFileFactory.Create(GetEasyType(idx), file, _config);

                    if (translate == null) { continue; }

                    _logger.Info($"translate system: {translate.GetType().Name} starting...");

                    tasks[idx] = Task.Run(() =>
                    {
                        using (LogicalThreadContext.Stacks["NDC"].Push($"Filename: {file}, {translate.GetType().Name}"))
                        {
                            try
                            {
                                translate.Translate();
                            }
                            finally { translate.Dispose(); }

                        }

                    }, _cancellationToken);

                    countRun++;

                }

                Task.WaitAll(tasks, _cancellationToken);

                PrintStatistics(statistics);

                //var tasks = new List<Task>
                //{
                //    Task.Run(() =>
                //        Parallel.For(0, d,
                //            new ParallelOptions
                //            {
                //                CancellationToken = _cancellationToken, MaxDegreeOfParallelism = MaxTasks
                //            }, (i) => TranslateDeeplFile(_files[i])), _cancellationToken),

                //    Task.Run(() =>
                //        Parallel.For(d, _files.Count-1,
                //            new ParallelOptions
                //            {
                //                CancellationToken = _cancellationToken, MaxDegreeOfParallelism = MaxTasks
                //            }, (i) => TranslateMtranslateByFile(_files[i])), _cancellationToken)
                //};

            }
            catch (Exception e)
            {
                _logger.Error($"translate files parallel failed: {e}");
            }
        }

        private void PrintStatistics(Dictionary<int, int> statistics)
        {
            _logger.Info($"translate statistics:");
            foreach (var statistic in statistics)
            {
                _logger.Info($"{GetEasyType(statistic.Key).Name}\t{statistic.Value}");
            }
        }

        private Dictionary<int, int> InitStatistics()
        {
            var statistics = new Dictionary<int, int>();
            for (var i = 0; i < _translateFiles.Length; i++)
            {
                statistics[i] = 0;
            }

            return statistics;
        }

        private Type GetEasyType(int index)
        {
            return _translateFiles[index];
        }
    }
}

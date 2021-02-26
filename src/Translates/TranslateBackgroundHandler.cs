using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using TranslateViaWeb.Configs;

namespace TranslateViaWeb.Translates
{
    /// <summary>
    /// Handler of queue with files
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
            typeof(TranslateRuFile)
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

                new TranslateRuFile(_files[0], _config).Translate();

                var tasks = new List<Task>(_translateFiles.Length);
                var cur = 0;

                foreach (var file in _files)
                {
                    if (tasks.Count >= _translateFiles.Length)
                    {
                        var idx = Task.WaitAny(tasks.ToArray(), _cancellationToken);
                        tasks.RemoveAt(idx);
                        cur = idx;
                    }

                    var translate = TranslateFileFactory.Create(_translateFiles[cur], file, _config);

                    tasks[cur] = Task.Run(() =>
                    {
                        using (LogicalThreadContext.Stacks["NDC"].Push($"Filename: {file}"))
                        {
                            translate.Translate();
                        }

                    }, _cancellationToken);
                }

                Task.WaitAll(tasks.ToArray(), _cancellationToken);

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

        /// <summary>
        /// Translate through deeple.com
        /// </summary>
        /// <param name="filename">File to translate</param>

        private void TranslateDeeplFile(string filename)
        {
            using var t = new DeeplTranslateFile(filename, _config);
            using (LogicalThreadContext.Stacks["NDC"].Push($"Filename: {filename}"))
            {
                t.Translate();
            }
        }

        /// <summary>
        /// Translate through M-translate.by
        /// </summary>
        /// <param name="filename">File to translate</param>
        private void TranslateMtranslateByFile(string filename)
        {
            using var t = new MTranslateByTranslateFile(filename, _config);
            using (LogicalThreadContext.Stacks["NDC"].Push($"Filename: {filename}"))
            {
                t.Translate();
            }
        }
    }
}

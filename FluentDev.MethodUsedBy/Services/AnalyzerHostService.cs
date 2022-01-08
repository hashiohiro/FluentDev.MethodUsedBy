using FluentDev.MethodUsedBy.Core;
using FluentDev.MethodUsedBy.Core.Error;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace FluentDev.MethodUsedBy.Services
{
    // https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice
    public class AnalyzerHostService : IHostedService
    {
        public AnalyzerHostService(IHostApplicationLifetime lifetime, IOptions<AppSettings> options, ILogger<AnalyzerHostService> logger, AnalyzeService main, ErrorHandler errorHandler)
        {
            this.Lifetime = lifetime;
            this.Settings = options.Value;
            this.Logger = logger;
            this.Main = main;
            this.ErrorHandler = errorHandler;
        }

        protected IHostApplicationLifetime Lifetime;
        protected ILogger<AnalyzerHostService> Logger;

        /// <summary>
        /// 構成ファイルモデル
        /// </summary>
        protected AppSettings Settings;
        protected AnalyzeService Main;
        protected ErrorHandler ErrorHandler;
        protected Stopwatch Stopwatch = new Stopwatch();

        /// <summary>
        /// ホスト起動時のイベント
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>非同期タスク</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Lifetime.ApplicationStarted.Register(this.Execute);
            this.Lifetime.ApplicationStopped.Register(this.Stopped);
        }

        /// <summary>
        /// サービス本体処理
        /// </summary>
        /// <returns>非同期タスク</returns>
        protected void Execute()
        {
            this.Stopwatch.Restart();
            this.Logger.LogInformation(MessageResource.MSG00001, this.Settings.AssemblyDir, this.Settings.Keyword);
            try
            {
                // 解析処理を実行する
                this.Main.Analyze();

                // 正常終了時の終了コードは「0」
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                // エラーメッセージを出力する
                try
                {
                    var errorMessages = this.ErrorHandler.GetErrorMessages(ex);

                    // ハンドル済み例外の場合
                    foreach (var each in errorMessages)
                    {
                        this.Logger.LogError(each);
                    }
                }
                // 未ハンドルの例外の場合
                catch (Exception unhandleEx)
                {
                    this.Logger.LogCritical(unhandleEx.ToString());
                }

                // エラー発生時の終了コードは「1」
                Environment.ExitCode = 1;
            }

            // アプリを終了する
            this.Lifetime.StopApplication();
        }

        protected void Stopped()
        {
            this.Stopwatch.Stop();
            this.Logger.LogInformation(MessageResource.MSG00013, this.Stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// ホスト終了時のイベント
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>非同期タスク</returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // nop
        }
    }
}

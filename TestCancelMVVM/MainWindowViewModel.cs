using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TestCancelMVVM
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<int> items = new();

        [ObservableProperty]
        public string textBlockStr = "任务未开始";

        CancellationTokenSource cancellationTokenSource = new();

        // 在 Test 方法的参数列表中加入 CancellationToken 类型的参数 cancellationToken，
        //并在 Task.Run 中使用 CancellationToken.Register 注册一个回调函数来取消任务。

        //在 WPF 应用程序中使用异步操作，则通常使用 TaskScheduler.FromCurrentSynchronizationContext()
        //方法来确保异步操作的结果在 UI 线程上下文中发生，以便正确更新用户界面。

        [RelayCommand(IncludeCancelCommand = true)]
        public async Task Test(CancellationToken cancellationToken)
        {
            try
            {
                Items.Clear();
                TextBlockStr = "开始任务！";

                await Task.Run(async () =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        // 检测CancellationToken中是否指示了取消操作
                        cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(1000);

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Items.Add(i);
                        });
                    }
                }, cancellationTokenSource.Token).ContinueWith((t) =>
                {
                    if (t.IsCanceled)
                    {
                        TextBlockStr = "任务已取消！";
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (TaskCanceledException)
            {
                TextBlockStr = "任务已取消！";
            }
            catch (Exception ex)
            {
                TextBlockStr = $"任务异常：{ex.Message}";
            }
        }

    }
}

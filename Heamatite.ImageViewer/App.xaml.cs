using Autofac;
using Heamatite.IO;
using Heamatite.IoSystem;
using Heamatite.View;
using Heamatite.View.Presenters;
using Heamatite.ViewInterfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Heamatite.ImageViewer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IContainer Container { get; set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<Configuration>().As<IConfiguration>().SingleInstance();
			builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();

			//views
			builder.RegisterType<ImageView>().As<IImageView>();
			builder.RegisterType<MainWindow>().As<IMainView>();

			//presenters
			builder.RegisterType<ImagePresenter>().As<IImagePresenter>();
			builder.RegisterType<MainPresenter>().As<IMainPresenter>();

			builder.RegisterType<MainWindowDirectoryWrapper>();
			builder.RegisterType<MainWindowFile>();

			builder.RegisterType<ImageFile>().As<IImageFile>();
			builder.RegisterType<ImageDirectory>().As<IImageDirectory>();

			builder.RegisterType<IoRepository>().As<IIoRepository>();

			var ioSystemAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(c => c.GetName().Name == "Heamatite.IoSystem");
			if (ioSystemAssembly == null)
			{
				throw new ApplicationException("can't file the Heamatite.IoSystem assembly");
			}
			builder.RegisterAssemblyTypes(ioSystemAssembly);

			Container = builder.Build();
			base.OnStartup(e);
			using (ILifetimeScope scope = Container.BeginLifetimeScope())
			{
				var winManager = scope.Resolve<IWindowManager>();
				winManager.ShowMainWindow();
			}
		}
	}
}

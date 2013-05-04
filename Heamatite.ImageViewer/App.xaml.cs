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
			BuildIocContainer();
			
			base.OnStartup(e);

			using (ILifetimeScope scope = Container.BeginLifetimeScope())
			{
				var winManager = scope.Resolve<IWindowManager>();
				winManager.ShowMainWindow();
			}
		}

		private static void BuildIocContainer()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<Configuration>().As<IConfiguration>().SingleInstance();
			builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();

			//presenters
			builder.Register(c =>
			{
				IImageView view = new ImageView();
				IImagePresenter presenter = new ImagePresenter(c.Resolve<IWindowManager>(), view, c.Resolve<IConfiguration>());
				return presenter;
			}).As<IImagePresenter>();
			builder.Register(c =>
			{
				IMainView view = new MainWindow();
				IMainPresenter presenter = new MainPresenter(c.Resolve<IWindowManager>(), 
					view, 
					c.Resolve<IConfiguration>(), 
					c.Resolve<IIoRepository>());
				return presenter;
			}).As<IMainPresenter>();

			//views
			builder.RegisterType<ImageView>().As<IImageView>();
			builder.RegisterType<MainWindow>().As<IMainView>();

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
		}
	}
}

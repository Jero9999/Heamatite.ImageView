using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Heamatite.IO
{
	public class FileRepository
	{
		public FileRepository()
		{
		}


		public string Location { get; set; }

		public IEnumerable<Io> GetContainerList()
		{
			Io io = Io.Create(Location);
			if (io == null)
			{
				return new Io[0];
			}
			if (!io.IoType.HasFlag(IoType.Directory))
			{
				throw new InvalidOperationException(Location + " is not a valid directory");
			}
			return (io as Directory).GetContents();
				
		}

		public void MoveToParent()
		{
			Location = Path.GetDirectoryName(Location);
		}
	}

}

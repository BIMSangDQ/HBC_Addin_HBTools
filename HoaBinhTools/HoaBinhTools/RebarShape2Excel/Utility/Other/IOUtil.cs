using System.IO;
using Autodesk.Revit.DB;

namespace Utility
{
	public static class IOUtil
	{
		public static bool IsFileInUse(FileInfo file)
		{
			bool rs = false;
			FileStream fs = null;
			try
			{
				fs = File.OpenWrite(file.FullName);
			}
			catch (IOException)
			{
				// the file is unvailabe because it is:
				// still being written to
				// or being processed by another thread
				// or does not exist (has already been processed)
				rs = true;
			}
			finally
			{
				if (fs != null)
					fs.Close();
			}

			return rs;
		}
		public static bool IsFileInUse(string fileName)
		{
			return IsFileInUse(new FileInfo(fileName));
		}

		/// <summary>
		/// Trả về đường dẫn thư mục chứa document đang xét
		/// </summary>
		/// <param name="doc">Document đang xét</param>
		/// <returns></returns>
		public static string GetDirectoryPath(Document doc)
		{
			return Path.GetDirectoryName(doc.PathName);
		}

		/// <summary>
		/// Trả về đường dẫn thư mục chưa document đang xét
		/// </summary>
		/// <param name="documentName">Tên document đang xét</param>
		/// <returns></returns>
		public static string GetDirectoryPath(string documentName)
		{
			return Path.GetDirectoryName(documentName);
		}

		/// <summary>
		/// Trả về đường dẫn mới chưa đường dẫn document đang xét, thêm vào tên và đuôi mở rộng mới
		/// </summary>
		/// <param name="doc">Document đang xét</param>
		/// <param name="name">Tên mới được thêm vào</param>
		/// <param name="exten">Đuôi mở rộng mới</param>
		/// <returns></returns>
		public static string CreateNameWithDocumentPathName
			(Document doc, string name, string exten)
		{
			string s = GetDirectoryPath(doc);
			string s1 = doc.PathName.Substring(s.Length + 1);
			return Path.Combine(s, s1.Substring(0, s1.Length - 4) + name + "." + exten);
		}

		/// <summary>
		/// Trả về đường dẫn mới chưa đường dẫn document đang xét, thêm vào tên và đuôi mở rộng mới
		/// </summary>
		/// <param name="doc">Tên document đang xét</param>
		/// <param name="name">Tên mới được thêm vào</param>
		/// <param name="exten">Đuôi mở rộng mới</param>
		/// <returns></returns>
		public static string CreateNameWithDocumentPathName
			(string documentName, string name, string exten)
		{
			string s = GetDirectoryPath(documentName);
			string s1 = documentName.Substring(s.Length + 1);
			return Path.Combine(s, s1.Substring(0, s1.Length - 4) + name + "." + exten);
		}
	}
}

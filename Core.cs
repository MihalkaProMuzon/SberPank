using System;

namespace SberPank
{
	public static class MessageConverter{
		
		const string delimitter = "______________________________________________________________";
		public static string[] convertToMessage(string filePath){
			var message = CdoWrapper.LoadMessage(filePath);
			
			string messageBody = "";
				
			string textBody = "Текстовая часть отсутвует";
			try { textBody = message.TextBodyPart.GetString(); } 
			catch {}
			
			string HTMLBody = "HTML часть отсутвует";
			try { HTMLBody = message.HTMLBodyPart.GetString(); } 
			catch {}
			
			messageBody = "Текстовое содержимое: "+ "\n\n" + textBody + '\n' + delimitter + '\n' + "HTML содержимое: " + "\n\n" + HTMLBody + '\n';
			
			return new []{message.Subject, messageBody};
		}
		
	}
	
	
	public class Field
	{
		public string name;
		
		public string From = "";
		public string To = "";
		
		public int offset = 0;
		public int index = 1;
		
		public bool checkRepeats = false;
		public bool optional = false;
		
		public string cellChars = "A";
		
		public Field(int index)
		{
			name = index.ToString();
		}
	}
}

using System;
using System.Text;

namespace Daishi.AMQP
{
	/// <summary>
	/// Base64Decoder에 대한 요약 설명입니다.
	/// </summary>
	public sealed class Base64Decoder : IDisposable
	{
		char[] source;
		int length, length2, length3;
		int blockCount;
		int paddingCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="Base64Decoder"/> class.
		/// </summary>
		/// <param name="input">The input.</param>
		public Base64Decoder(char[] input) {
			int temp = 0;
			source = input;
			length = input.Length;

			//find how many padding are there
			for (int x = 0; x < 2; x++) {
				if (input[length - x - 1] == '=')
					temp++;
			}
			paddingCount = temp;
			//calculate the blockCount;
			//assuming all whitespace and carriage returns/newline were removed.
			blockCount = length / 4;
			length2 = blockCount * 3;
		}

		/// <summary>
		/// Gets the decoded.
		/// </summary>
		/// <returns></returns>
		public byte[] GetDecoded() {
			byte[] buffer = new byte[length];//first conversion result
			byte[] buffer2 = new byte[length2];//decoded array with padding

			for (int x = 0; x < length; x++) {
				buffer[x] = char2sixbit(source[x]);
			}

			byte b, b1, b2, b3;
			byte temp1, temp2, temp3, temp4;

			for (int x = 0; x < blockCount; x++) {
				temp1 = buffer[x * 4];
				temp2 = buffer[x * 4 + 1];
				temp3 = buffer[x * 4 + 2];
				temp4 = buffer[x * 4 + 3];

				b = (byte)(temp1 << 2);
				b1 = (byte)((temp2 & 48) >> 4);
				b1 += b;

				b = (byte)((temp2 & 15) << 4);
				b2 = (byte)((temp3 & 60) >> 2);
				b2 += b;

				b = (byte)((temp3 & 3) << 6);
				b3 = temp4;
				b3 += b;

				buffer2[x * 3] = b1;
				buffer2[x * 3 + 1] = b2;
				buffer2[x * 3 + 2] = b3;
			}
			//remove paddings
			length3 = length2 - paddingCount;
			byte[] result = new byte[length3];

			for (int x = 0; x < length3; x++) {
				result[x] = buffer2[x];
			}

			return result;
		}

		/// <summary>
		/// Char2sixbits the specified c.
		/// </summary>
		/// <param name="c">The c.</param>
		/// <returns></returns>
		private byte char2sixbit(char c) {
			char[] lookupTable = new char[64]
			{  
			  'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
			  'O','P','Q','R','S','T','U','V','W','X','Y', 'Z',
			  'a','b','c','d','e','f','g','h','i','j','k','l','m','n',
			  'o','p','q','r','s','t','u','v','w','x','y','z',
			  '0','1','2','3','4','5','6','7','8','9','+','/'};

			if (c == '=')
				return 0;
			else {
				for (int x = 0; x < 64; x++) {
					if (lookupTable[x] == c)
						return (byte)x;
				}
				//should not reach here
				return 0;
			}
		}

		/// <summary>
		/// Gets the base64 decoded.
		/// </summary>
		/// <param name="src">The SRC.</param>
		/// <returns></returns>
		public static string GetBase64Decoded(string src) {
			char[] data = src.ToCharArray();
			Base64Decoder myDecoder = new Base64Decoder(data);
			StringBuilder sb = new StringBuilder();

			byte[] temp = myDecoder.GetDecoded();
			sb.Append(System.Text.UTF8Encoding.UTF8.GetChars(temp));

			return sb.ToString();
		}
		#region IDisposable Members
		private bool m_bDisposed = false;
		/// <summary>
		/// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 응용 프로그램 정의 작업을 수행합니다.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="bDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool bDisposing) {
			// Check to see if Dispose has already been called.
			if (!this.m_bDisposed) {
				// If disposing equals true, dispose all managed and unmanaged resources.
				if (bDisposing) {
					// Dispose managed resources.
				}
				// Dispose unmanaged resources.

				m_bDisposed = true;// Note disposing has been done.
			}
		}
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Base64Decoder"/> is reclaimed by garbage collection.
		/// </summary>
		~Base64Decoder() {
			Dispose(false);
		}
		#endregion
	}
}

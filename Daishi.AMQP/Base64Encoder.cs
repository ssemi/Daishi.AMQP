using System;
using System.Text;

namespace Daishi.AMQP
{
	/// <summary>
	/// Base64Encoder에 대한 요약 설명입니다.
	/// </summary>
	public sealed class Base64Encoder : IDisposable
	{
		byte[] source;
		int length, length2;
		int blockCount;
		int paddingCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="Base64Encoder"/> class.
		/// </summary>
		/// <param name="input">The input.</param>
		public Base64Encoder(byte[] input) {
			source = input;
			length = input.Length;
			if ((length % 3) == 0) {
				paddingCount = 0;
				blockCount = length / 3;
			} else {
				paddingCount = 3 - (length % 3);//need to add padding
				blockCount = (length + paddingCount) / 3;
			}
			length2 = length + paddingCount;//or blockCount *3
		}

		/// <summary>
		/// Gets the encoded.
		/// </summary>
		/// <returns></returns>
		public char[] GetEncoded() {
			byte[] source2;
			source2 = new byte[length2];
			//copy data over insert padding
			for (int x = 0; x < length2; x++) {
				if (x < length) {
					source2[x] = source[x];
				} else {
					source2[x] = 0;
				}
			}

			byte b1, b2, b3;
			byte temp, temp1, temp2, temp3, temp4;
			byte[] buffer = new byte[blockCount * 4];
			char[] result = new char[blockCount * 4];
			for (int x = 0; x < blockCount; x++) {
				b1 = source2[x * 3];
				b2 = source2[x * 3 + 1];
				b3 = source2[x * 3 + 2];

				temp1 = (byte)((b1 & 252) >> 2);//first

				temp = (byte)((b1 & 3) << 4);
				temp2 = (byte)((b2 & 240) >> 4);
				temp2 += temp; //second

				temp = (byte)((b2 & 15) << 2);
				temp3 = (byte)((b3 & 192) >> 6);
				temp3 += temp; //third

				temp4 = (byte)(b3 & 63); //fourth

				buffer[x * 4] = temp1;
				buffer[x * 4 + 1] = temp2;
				buffer[x * 4 + 2] = temp3;
				buffer[x * 4 + 3] = temp4;

			}

			for (int x = 0; x < blockCount * 4; x++) {
				result[x] = sixbit2char(buffer[x]);
			}

			//covert last "A"s to "=", based on paddingCount
			switch (paddingCount) {
				case 0: break;
				case 1: result[blockCount * 4 - 1] = '='; break;
				case 2: result[blockCount * 4 - 1] = '=';
					result[blockCount * 4 - 2] = '=';
					break;
				default: break;
			}
			return result;
		}

		private char sixbit2char(byte b) {
			char[] lookupTable = new char[64]
	  {
		  'A','B','C','D','E','F','G','H','I','J','K','L','M',
		  'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
		  'a','b','c','d','e','f','g','h','i','j','k','l','m',
		  'n','o','p','q','r','s','t','u','v','w','x','y','z',
		  '0','1','2','3','4','5','6','7','8','9','+','/'};

			if ((b >= 0) && (b <= 63)) {
				return lookupTable[(int)b];
			} else {
				//should not happen;
				return ' ';
			}
		}

		/// <summary>
		/// Gets the base64 encoded.
		/// </summary>
		/// <param name="src">The SRC.</param>
		/// <returns></returns>
		public static string GetBase64Encoded(string src) {
			//first get the bytes for the original
			byte[] data = System.Text.UnicodeEncoding.UTF8.GetBytes(src);
			Base64Encoder myEncoder = new Base64Encoder(data);
			StringBuilder sb = new StringBuilder();

			sb.Append(myEncoder.GetEncoded());

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
		/// <see cref="Base64Encoder"/> is reclaimed by garbage collection.
		/// </summary>
		~Base64Encoder() {
			Dispose(false);
		}
		#endregion
	}

}

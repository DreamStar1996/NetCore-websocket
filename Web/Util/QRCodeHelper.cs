using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Util
{
    /// <summary>
    /// 二维码工具类
    /// <see href="https://github.com/codebude/QRCoder"/>
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">扫描二维码时显示的文本内容，如果是网址自动跳转</param>
        /// <param name="pixel">像素</param>
        /// <returns></returns>
        public static Bitmap GetQRCode(string text, int pixel)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData codeData = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M, true);
            QRCode qrcode = new QRCode(codeData);
            Bitmap qrImage = qrcode.GetGraphic(pixel, Color.Black, Color.White, true);
            return qrImage;
        }
    }
}

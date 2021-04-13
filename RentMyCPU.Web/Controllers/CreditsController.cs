using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentMyCPU.Backend.Data;
using RentMyCPU.Backend.Data.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentMyCPU.Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CreditsController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private readonly UserManager<User> _userManager;

        public CreditsController(ApplicationDbContext applicationDbContext, UserManager<User> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        [Route("/fulfilled/{transactionId}")]
        public async Task<IActionResult> IsFulfilled([FromRoute] Guid transactionId)
        {
            var userId = await _applicationDbContext.Users
                .Where(x => x.Email == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(x => x.Id)
                .SingleOrDefaultAsync();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            return await _applicationDbContext.Purchases.AnyAsync(x => x.TransactionId == transactionId && x.UserId == userId)
                ? (IActionResult)Ok()
                : BadRequest();
        }

        [Route("{transactionId}")]
        [HttpPost]
        public async Task<IActionResult> AddCredits([FromRoute] Guid transactionId)
        {
            string receipt = GetRequestContentAsString();
#if DEBUG
            await AddCreditsInternal(transactionId, receipt);
            return Ok();
#else 
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new MemoryStream(Encoding.UTF8.GetBytes(receipt)));

            XmlNode node = xmlDoc.DocumentElement;
            string certificateId = node.Attributes["CertificateId"].Value;

            if (string.IsNullOrEmpty(certificateId))
                return BadRequest();

            X509Certificate2 verificationCertificate = await RetrieveCertificate(certificateId);
            if (verificationCertificate == null) 
                return BadRequest();

            bool isValid = ValidateXml(xmlDoc, verificationCertificate);

            if (!isValid)
                return BadRequest();

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("ProductReceipt");
            if (xmlNodeList.Count != 1)
                return BadRequest();

            var productId = xmlNodeList[0].Attributes["ProductId"].Value;
            if (productId != "Credits")
                return BadRequest();

            await AddCreditsInternal(transactionId, receipt);
            return Ok();
#endif
        }

        private string GetRequestContentAsString()
        {
            using (var receiveStream = Request.Body)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
        }
        private async Task AddCreditsInternal(Guid transactionId, string receipt)
        {
            var user = await _applicationDbContext.Users
                                .Where(x => x.Email == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                .SingleOrDefaultAsync();

            user.Credits += 70;

            var purchase = new Purchase
            {
                TransactionId = transactionId,
                UserId = user.Id,
                Receipt = receipt
            };

            _applicationDbContext.Purchases.Add(purchase);
            await _applicationDbContext.SaveChangesAsync();
        }

        public static async Task<X509Certificate2> RetrieveCertificate(string certificateId)
        {
            const int MaxCertificateSize = 10000;

            // Retrieve the certificate URL.
            String certificateUrl = String.Format(
                "https://go.microsoft.com/fwlink/?LinkId=246509&cid={0}", certificateId);

            // Make an HTTP GET request for the certificate
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(certificateUrl);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());

            // Retrieve the certificate out of the response stream
            byte[] responseBuffer = new byte[MaxCertificateSize];
            Stream resStream = response.GetResponseStream();
            int bytesRead = ReadResponseBytes(responseBuffer, resStream);

            if (bytesRead < 1)
            {
                return null;
            }

            return new X509Certificate2(responseBuffer);
        }


        private static int ReadResponseBytes(byte[] responseBuffer, Stream resStream)
        {
            int count = 0;
            int numBytesRead = 0;
            int numBytesToRead = responseBuffer.Length;

            do
            {
                count = resStream.Read(responseBuffer, numBytesRead, numBytesToRead);
                numBytesRead += count;
                numBytesToRead -= count;
            } while (count > 0);

            return numBytesRead;
        }

        static bool ValidateXml(XmlDocument receipt, X509Certificate2 certificate)
        {
            // Create the signed XML object.
            SignedXml sxml = new SignedXml(receipt);

            // Get the XML Signature node and load it into the signed XML object.
            XmlNode dsig = receipt.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
            if (dsig == null)
            {
                // If signature is not found return false 
                return false;
            }

            sxml.LoadXml((XmlElement)dsig);

            // Check the signature
            bool isValid = sxml.CheckSignature(certificate, true);

            return isValid;
        }

    }

}

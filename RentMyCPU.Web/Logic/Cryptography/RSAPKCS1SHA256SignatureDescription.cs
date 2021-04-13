﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Logic.Cryptography
{
    public sealed class RSAPKCS1SHA256SignatureDescription : SignatureDescription
    {
        public RSAPKCS1SHA256SignatureDescription()
        {
            base.KeyAlgorithm = typeof(RSACryptoServiceProvider).FullName;
            base.DigestAlgorithm = typeof(SHA256Managed).FullName;
            base.FormatterAlgorithm = typeof(RSAPKCS1SignatureFormatter).FullName;
            base.DeformatterAlgorithm = typeof(RSAPKCS1SignatureDeformatter).FullName;
        }

        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
            deformatter.SetHashAlgorithm("SHA256");
            return deformatter;
        }

        public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("SHA256");
            return formatter;
        }
    }

}

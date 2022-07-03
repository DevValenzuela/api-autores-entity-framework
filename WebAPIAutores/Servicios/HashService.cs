using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Servicios
{
    public class HashService
    {

        public ResultadoHash Hash(string TextPlano)
        {
            var sal = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);
            }
            return Hash(TextPlano, sal);

        }

        public ResultadoHash Hash(string textPlano, byte[] sal)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(password: textPlano,
                salt: sal, prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(llaveDerivada);
            return new ResultadoHash()
            {
                Hash = hash,
                Sal = sal
            };
        }
    }
}

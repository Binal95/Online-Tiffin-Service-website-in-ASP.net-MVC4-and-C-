using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
//using System.Windows.Forms;

/// <summary>
/// Summary description for MD5Crypt
/// </summary>
public class MD5Crypt
{
	public MD5Crypt()
	{
		
	}
   
    /// <summary>
    /// Enables encryption of strings using the MD5 algorithm.
    /// </summary>
           /// <summary>
        /// Encrypts a string using a specified security key with
        /// the option to hash.
        /// </summary>
        /// <param name="toEncrypt">String to encrypt</param>
        /// <param name="securityKey">The key to apply to the encryption</param>
        /// <param name="useHashing">Weather hashing is used</param>
        /// <returns>The encrpyted string</returns>
        public static string Encrypt(string toEncrypt, string securityKey)
        {
            string retVal = string.Empty;
 
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
 
       
                // If hashing use get hashcode regards to your key
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));
 
                    // Always release the resources and flush data
                    // of the Cryptographic service provide. Best Practice
                    hashmd5.Clear();
                
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
 
                // Set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
 
                // Mode of operation. there are other 4 modes.
                // We choose ECB (Electronic code Book)
                tdes.Mode = CipherMode.ECB;
 
                // Padding mode (if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;
 
                ICryptoTransform cTransform = tdes.CreateEncryptor();
 
                // Transform the specified region of bytes array to resultArray
                byte[] resultArray =
                  cTransform.TransformFinalBlock(toEncryptArray, 0,
                  toEncryptArray.Length);
 
                // Release resources held by TripleDes Encryptor
                tdes.Clear();
 
                // Return the encrypted data into unreadable string format
                retVal = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Error!");     
            }
     
            return retVal;
        }
 
        /// <summary>
        /// Decrypts a specified key against the original security
        /// key, with the option to hash.
        /// </summary>
        /// <param name="cipherString">String to decrypt</param>
        /// <param name="securityKey">The original security key</param>
        /// <param name="useHashing">Weather hashing is enabled</param>
        /// <returns>The decrypted key</returns>
        public static string Decrypt(string cipherString, string securityKey)
        {
            string retVal = string.Empty;
 
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);
 
       
                    // If hashing was used get the hash code with regards to your key
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));
 
                    // Release any resource held by the MD5CryptoServiceProvider
                    hashmd5.Clear();
                
 
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
 
                // Set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
 
                // Mode of operation. there are other 4 modes. 
                // We choose ECB(Electronic code Book)
                tdes.Mode = CipherMode.ECB;
 
                // Padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;
 
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(
                                     toEncryptArray, 0, toEncryptArray.Length);
 
                // Release resources held by TripleDes Encryptor
                tdes.Clear();
 
                // Return the Clear decrypted TEXT
                retVal = UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
              // MessageBox.Show("Error!");
            }
 
            return retVal;
        }
 
       
    }
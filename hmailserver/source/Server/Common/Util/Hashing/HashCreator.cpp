// Copyright (c) 2010 Martin Knafve / hMailServer.com.  
// http://www.hmailserver.com

#include <stdafx.h>

#include "../PasswordGenerator.h"
#include "../../Mime/MimeCode.h"

#include <openssl/sha.h>
#include <openssl/evp.h>
#include <openssl/md5.h>

#include "HashCreator.h"

#ifdef _DEBUG
#define DEBUG_NEW new(_NORMAL_BLOCK, __FILE__, __LINE__)
#define new DEBUG_NEW
#endif

namespace HM
{
   HashCreator::HashCreator(HashCreator::HashType hashType) :
      hash_type_(hashType)
   {

   }

   AnsiString HashCreator::GenerateHash(const AnsiString &inputString, const AnsiString &salt)
   {
      AnsiString saltString = salt;
      if (saltString.GetLength() == 0 && hash_type_ == SHA256)
      {     
         AnsiString randomString = PasswordGenerator::Generate();
         saltString = GetHash_(randomString, hex);
         saltString = saltString.Mid(0, SALT_LENGTH);
      }

      AnsiString value = saltString + GetHash_(saltString + inputString, hex);
      return value;
   }

   AnsiString 
   HashCreator::GenerateHashNoSalt(const AnsiString &inputString, RequestedEncoding encoding)
   {
      return GetHash_(inputString, encoding);
   }

   AnsiString 
   HashCreator::GenerateHashNoSalt(unsigned char *input, int inputLength, RequestedEncoding encoding)
   {
      return GetHash_Raw(input, inputLength, encoding);
   }


   bool 
   HashCreator::ValidateHash(const AnsiString &password, const AnsiString &originalHash, bool useSalt)
   {
      if (useSalt)
      {
         AnsiString salt = GetSalt_(originalHash);
         AnsiString result = GenerateHash(password, salt);

         if (result == originalHash)
            return true;
         else
            return false;
      }
      else
      {
         AnsiString result = GetHash_(password, hex);

         if (result == originalHash)
            return true;
         else
            return false;
      }


   }

   AnsiString HashCreator::GetSalt_(const AnsiString &inputString)
   {
      AnsiString result = inputString.Mid(0,SALT_LENGTH);
      return result;
   }

   AnsiString HashCreator::GetHash_(const AnsiString &sInputString, HashCreator::RequestedEncoding encoding)
   {
      AnsiString temp = sInputString;
      return GetHash_Raw((unsigned char*) temp.GetBuffer(), temp.GetLength(), encoding);
   }

   AnsiString HashCreator::GetHash_Raw(const unsigned char *input, int inputLength, HashCreator::RequestedEncoding encoding)
   {
      int digestLength = 0;

      switch (hash_type_)
      {
      case SHA1:
         digestLength = SHA_DIGEST_LENGTH;
         break;
      case SHA256:
         digestLength = SHA256_DIGEST_LENGTH;
         break;
      case MD5:
         digestLength = MD5_DIGEST_LENGTH;
         break;
      }

      unsigned char *results = new unsigned char[digestLength];

      EVP_MD_CTX *context = EVP_MD_CTX_new();
      const EVP_MD *md = EVP_md_null();

      switch (hash_type_)
      {
      case SHA1:
         md = EVP_sha1();
         break;
      case MD5:
         md = EVP_md5();
         break;
      case SHA256:
         md = EVP_sha256();
         break;
      }

      EVP_DigestInit_ex(context, md, NULL);
      EVP_DigestUpdate(context, input, inputLength);
      EVP_DigestFinal_ex(context, results, NULL);
      EVP_MD_CTX_free(context);


      HM::AnsiString retVal;
      if (encoding == hex)
      {
         char buffer[3];
         buffer[2] = '\0';

         for (int i = 0; i < digestLength; i++)
         {
            sprintf_s(buffer, 3, "%02x", results[i]);

            retVal += buffer;
         }

      }
      else if (encoding == base64)
      {
         MimeCodeBase64 encoder;
         encoder.SetInput((const char*) results, digestLength, true);

         AnsiString sEncodedValue;
         encoder.GetOutput(sEncodedValue);

         retVal = sEncodedValue;
         retVal = retVal.Mid(0, retVal.GetLength()-2);
      }

      delete [] results;

      return retVal;
   }

   void
   HashCreatorTester::Test()
   {
      // Run basic test.
      HashCreator hasher(HashCreator::SHA256);
      AnsiString result = hasher.GenerateHash("The quick brown fox jumps over the lazy dog", "");

      if (!hasher.ValidateHash("The quick brown fox jumps over the lazy dog", result, true))
         throw 0;

      // Check that same password hashed twice yealds separate hashes.
      AnsiString test1 = hasher.GenerateHash("The quick brown fox jumps over the lazy dog", "");
      AnsiString test2 = hasher.GenerateHash("The quick brown fox jumps over the lazy dog", "");
      if (test1 == test2)
         throw 0;

      for (int i = 0; i < 250; i++)
      {
         HashCreator memoryTester(HashCreator::SHA256);

         String temp;
         temp.Format(_T("%d"), i);
         AnsiString hashableString = temp;

         hasher.GenerateHash(hashableString, "test");
      }
   }
}
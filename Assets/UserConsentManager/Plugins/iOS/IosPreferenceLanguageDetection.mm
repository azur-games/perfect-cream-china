char* cStringC(const char* string)
{
   if (string == NULL)
   {
       return NULL;
   }
   char* res = (char*)malloc(strlen(string) + 1);
   strcpy(res, string);
   return res;
}
extern "C"
{
    // -- we define our external method to be in C.
    char* _getPreferenceLanguageString()
    {
    NSString * language =[[NSLocale preferredLanguages]firstObject];
    return cStringC([language UTF8String]);
    }
}

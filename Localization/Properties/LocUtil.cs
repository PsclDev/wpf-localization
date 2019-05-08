using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Localization.Properties
{
    public static class LocUtil
    {
        /// <summary>  
        /// Get application name from an element  
        /// </summary>  
        /// <param name="element"></param>  
        /// <returns></returns>  
        private static string getAppName(FrameworkElement element)
        {
            var elType = element.GetType().ToString();
            var elNames = elType.Split('.');
            return elNames[0];
        }


        /// <summary>  
        /// Generate a name from an element base on its class name  
        /// </summary>  
        /// <param name="element"></param>  
        /// <returns></returns>  
        private static string getElementName(FrameworkElement element)
        {
            var elType = element.GetType().ToString();
            var elNames = elType.Split('.');
            var elName = "";
            if (elNames.Length >= 2) elName = elNames[elNames.Length - 1];
            return elName;
        }


        /// <summary>  
        /// Get current culture info name base on previously saved setting if any,  
        /// otherwise get from OS language  
        /// </summary>  
        /// <param name="element"></param>  
        /// <returns></returns>  
        public static string GetCurrentCultureName(FrameworkElement element)
        {
            RegistryKey curLocInfo = Registry.CurrentUser.OpenSubKey("GsmLib" + @"\" + getAppName(element), false);
            var cultureName = CultureInfo.CurrentUICulture.Name;
            if (curLocInfo != null)
            {
                cultureName = curLocInfo.GetValue(getElementName(element) + ".localization", "en-US").ToString();
            }
            return cultureName;
        }


        /// <summary>  
        /// Set language based on previously save language setting,  
        /// otherwise set to OS lanaguage  
        /// </summary>  
        /// <param name="element"></param>  
        public static void SetDefaultLanguage(FrameworkElement element)
        {
            SetLanguageResourceDictionary(element, GetLocXAMLFilePath(getElementName(element), GetCurrentCultureName(element)));
        }


        /// <summary>  
        /// Dynamically load a Localization ResourceDictionary from a file  
        /// </summary>  
        public static void SwitchLanguage(FrameworkElement element, string inFiveCharLang)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(inFiveCharLang);
            SetLanguageResourceDictionary(element, GetLocXAMLFilePath(getElementName(element), inFiveCharLang));
            // Save new culture info to registry  
            RegistryKey UserPrefs = Registry.CurrentUser.OpenSubKey("GsmLib" + @"\" + getAppName(element), true);
            if (UserPrefs == null)
            {
                // Value does not already exist so create it  
                RegistryKey newKey = Registry.CurrentUser.CreateSubKey("GsmLib");
                UserPrefs = newKey.CreateSubKey(getAppName(element));
            }
            UserPrefs.SetValue(getElementName(element) + ".localization", inFiveCharLang);
        }


        /// <summary>  
        /// Returns the path to the ResourceDictionary file based on the language character string.  
        /// </summary>  
        /// <param name="inFiveCharLang"></param>  
        /// <returns></returns>  
        public static string GetLocXAMLFilePath(string element, string inFiveCharLang)
        {
            string locXamlFile = element + "." + inFiveCharLang + ".xaml";
            string directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(directory, "i18N", locXamlFile);
        }


        /// <summary>  
        /// Sets or replaces the ResourceDictionary by dynamically loading  
        /// a Localization ResourceDictionary from the file path passed in.  
        /// </summary>  
        /// <param name="inFile"></param>  
        private static void SetLanguageResourceDictionary(FrameworkElement element, String inFile)
        {
            if (File.Exists(inFile))
            {
                // Read in ResourceDictionary File  
                var languageDictionary = new ResourceDictionary();
                languageDictionary.Source = new Uri(inFile);
                // Remove any previous Localization dictionaries loaded  
                int langDictId = -1;
                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var md = element.Resources.MergedDictionaries[i];
                    // Make sure your Localization ResourceDictionarys have the ResourceDictionaryName  
                    // key and that it is set to a value starting with "Loc-".  
                    if (md.Contains("ResourceDictionaryName"))
                    {
                        if (md["ResourceDictionaryName"].ToString().StartsWith("Loc-"))
                        {
                            langDictId = i;
                            break;
                        }
                    }
                }
                if (langDictId == -1)
                {
                    // Add in newly loaded Resource Dictionary  
                    element.Resources.MergedDictionaries.Add(languageDictionary);
                }
                else
                {
                    // Replace the current langage dictionary with the new one  
                    element.Resources.MergedDictionaries[langDictId] = languageDictionary;
                }
            }
            else
            {
                MessageBox.Show("'" + inFile + "' not found.");
            }
        }

        public static List<ComboBoxItem> LanguageList = new List<ComboBoxItem>()
        {
            new ComboBoxItem() {Content = "Arabic (Saudi Arabia)", Tag = "ar-SA"},
            new ComboBoxItem() {Content = "Bulgarian (Bulgaria)", Tag = "bg-BG"},
            new ComboBoxItem() {Content = "Catalan (Catalan)", Tag = "ca-ES"},
            new ComboBoxItem() {Content = "Chinese (Taiwan)", Tag = "zh-TW"},
            new ComboBoxItem() {Content = "Czech (Czech Republic)", Tag = "cs-CZ"},
            new ComboBoxItem() {Content = "Danish (Denmark)", Tag = "da-DK"},
            new ComboBoxItem() {Content = "German (Germany)", Tag = "de-DE"},
            new ComboBoxItem() {Content = "Greek (Greece)", Tag = "el-GR"},
            new ComboBoxItem() {Content = "English (United States)", Tag = "en-US"},
            new ComboBoxItem() {Content = "Finnish (Finland)", Tag = "fi-FI"},
            new ComboBoxItem() {Content = "French (France)", Tag = "fr-FR"},
            new ComboBoxItem() {Content = "Hebrew (Israel)", Tag = "he-IL"},
            new ComboBoxItem() {Content = "Hungarian (Hungary)", Tag = "hu-HU"},
            new ComboBoxItem() {Content = "Icelandic (Iceland)", Tag = "is-IS"},
            new ComboBoxItem() {Content = "Italian (Italy)", Tag = "it-IT"},
            new ComboBoxItem() {Content = "Japanese (Japan)", Tag = "ja-JP"},
            new ComboBoxItem() {Content = "Korean (Korea)", Tag = "ko-KR"},
            new ComboBoxItem() {Content = "Dutch (Netherlands)", Tag = "nl-NL"},
            new ComboBoxItem() {Content = "Norwegian, BokmÃ¥l (Norway)", Tag = "nb-NO"},
            new ComboBoxItem() {Content = "Polish (Poland)", Tag = "pl-PL"},
            new ComboBoxItem() {Content = "Portuguese (Brazil)", Tag = "pt-BR"},
            new ComboBoxItem() {Content = "Romanian (Romania)", Tag = "ro-RO"},
            new ComboBoxItem() {Content = "Russian (Russia)", Tag = "ru-RU"},
            new ComboBoxItem() {Content = "Croatian (Croatia)", Tag = "hr-HR"},
            new ComboBoxItem() {Content = "Slovak (Slovakia)", Tag = "sk-SK"},
            new ComboBoxItem() {Content = "Albanian (Albania)", Tag = "sq-AL"},
            new ComboBoxItem() {Content = "Swedish (Sweden)", Tag = "sv-SE"},
            new ComboBoxItem() {Content = "Thai (Thailand)", Tag = "th-TH"},
            new ComboBoxItem() {Content = "Turkish (Turkey)", Tag = "tr-TR"},
            new ComboBoxItem() {Content = "Urdu (Islamic Republic of Pakistan)", Tag = "ur-PK"},
            new ComboBoxItem() {Content = "Indonesian (Indonesia)", Tag = "id-ID"},
            new ComboBoxItem() {Content = "Ukrainian (Ukraine)", Tag = "uk-UA"},
            new ComboBoxItem() {Content = "Belarusian (Belarus)", Tag = "be-BY"},
            new ComboBoxItem() {Content = "Slovenian (Slovenia)", Tag = "sl-SI"},
            new ComboBoxItem() {Content = "Estonian (Estonia)", Tag = "et-EE"},
            new ComboBoxItem() {Content = "Latvian (Latvia)", Tag = "lv-LV"},
            new ComboBoxItem() {Content = "Lithuanian (Lithuania)", Tag = "lt-LT"},
            new ComboBoxItem() {Content = "Persian (Iran)", Tag = "fa-IR"},
            new ComboBoxItem() {Content = "Vietnamese (Vietnam)", Tag = "vi-VN"},
            new ComboBoxItem() {Content = "Armenian (Armenia)", Tag = "hy-AM"},
            new ComboBoxItem() {Content = "Azeri (Latin, Azerbaijan)", Tag = "az-Latn-AZ"},
            new ComboBoxItem() {Content = "Basque (Basque)", Tag = "eu-ES"},
            new ComboBoxItem() {Content = "Macedonian (Former Yugoslav Republic of Macedonia)", Tag = "mk-MK"},
            new ComboBoxItem() {Content = "Afrikaans (South Africa)", Tag = "af-ZA"},
            new ComboBoxItem() {Content = "Georgian (Georgia)", Tag = "ka-GE"},
            new ComboBoxItem() {Content = "Faroese (Faroe Islands)", Tag = "fo-FO"},
            new ComboBoxItem() {Content = "Hindi (India)", Tag = "hi-IN"},
            new ComboBoxItem() {Content = "Malay (Malaysia)", Tag = "ms-MY"},
            new ComboBoxItem() {Content = "Kazakh (Kazakhstan)", Tag = "kk-KZ"},
            new ComboBoxItem() {Content = "Kyrgyz (Kyrgyzstan)", Tag = "ky-KG"},
            new ComboBoxItem() {Content = "Kiswahili (Kenya)", Tag = "sw-KE"},
            new ComboBoxItem() {Content = "Uzbek (Latin, Uzbekistan)", Tag = "uz-Latn-UZ"},
            new ComboBoxItem() {Content = "Tatar (Russia)", Tag = "tt-RU"},
            new ComboBoxItem() {Content = "Punjabi (India)", Tag = "pa-IN"},
            new ComboBoxItem() {Content = "Gujarati (India)", Tag = "gu-IN"},
            new ComboBoxItem() {Content = "Tamil (India)", Tag = "ta-IN"},
            new ComboBoxItem() {Content = "Telugu (India)", Tag = "te-IN"},
            new ComboBoxItem() {Content = "Kannada (India)", Tag = "kn-IN"},
            new ComboBoxItem() {Content = "Marathi (India)", Tag = "mr-IN"},
            new ComboBoxItem() {Content = "Sanskrit (India)", Tag = "sa-IN"},
            new ComboBoxItem() {Content = "Mongolian (Cyrillic, Mongolia)", Tag = "mn-MN"},
            new ComboBoxItem() {Content = "Galician (Galician)", Tag = "gl-ES"},
            new ComboBoxItem() {Content = "Konkani (India)", Tag = "kok-IN"},
            new ComboBoxItem() {Content = "Syriac (Syria)", Tag = "syr-SY"},
            new ComboBoxItem() {Content = "Divehi (Maldives)", Tag = "dv-MV"},
            new ComboBoxItem() {Content = "Arabic (Iraq)", Tag = "ar-IQ"},
            new ComboBoxItem() {Content = "Chinese (People's Republic of China)", Tag = "zh-CN"},
            new ComboBoxItem() {Content = "German (Switzerland)", Tag = "de-CH"},
            new ComboBoxItem() {Content = "English (United Kingdom)", Tag = "en-GB"},
            new ComboBoxItem() {Content = "Spanish (Mexico)", Tag = "es-MX"},
            new ComboBoxItem() {Content = "French (Belgium)", Tag = "fr-BE"},
            new ComboBoxItem() {Content = "Italian (Switzerland)", Tag = "it-CH"},
            new ComboBoxItem() {Content = "Dutch (Belgium)", Tag = "nl-BE"},
            new ComboBoxItem() {Content = "Norwegian, Nynorsk (Norway)", Tag = "nn-NO"},
            new ComboBoxItem() {Content = "Portuguese (Portugal)", Tag = "pt-PT"},
            new ComboBoxItem() {Content = "Serbian (Latin, Serbia)", Tag = "sr-Latn-CS"},
            new ComboBoxItem() {Content = "Swedish (Finland)", Tag = "sv-FI"},
            new ComboBoxItem() {Content = "Azeri (Cyrillic, Azerbaijan)", Tag = "az-Cyrl-AZ"},
            new ComboBoxItem() {Content = "Malay (Brunei Darussalam)", Tag = "ms-BN"},
            new ComboBoxItem() {Content = "Uzbek (Cyrillic, Uzbekistan)", Tag = "uz-Cyrl-UZ"},
            new ComboBoxItem() {Content = "Arabic (Egypt)", Tag = "ar-EG"},
            new ComboBoxItem() {Content = "Chinese (Hong Kong S.A.R.)", Tag = "zh-HK"},
            new ComboBoxItem() {Content = "German (Austria)", Tag = "de-AT"},
            new ComboBoxItem() {Content = "English (Australia)", Tag = "en-AU"},
            new ComboBoxItem() {Content = "Spanish (Spain)", Tag = "es-ES"},
            new ComboBoxItem() {Content = "French (Canada)", Tag = "fr-CA"},
            new ComboBoxItem() {Content = "Serbian (Cyrillic, Serbia)", Tag = "sr-Cyrl-CS"},
            new ComboBoxItem() {Content = "Arabic (Libya)", Tag = "ar-LY"},
            new ComboBoxItem() {Content = "Chinese (Singapore)", Tag = "zh-SG"},
            new ComboBoxItem() {Content = "German (Luxembourg)", Tag = "de-LU"},
            new ComboBoxItem() {Content = "English (Canada)", Tag = "en-CA"},
            new ComboBoxItem() {Content = "Spanish (Guatemala)", Tag = "es-GT"},
            new ComboBoxItem() {Content = "French (Switzerland)", Tag = "fr-CH"},
            new ComboBoxItem() {Content = "Arabic (Algeria)", Tag = "ar-DZ"},
            new ComboBoxItem() {Content = "Chinese (Macao S.A.R.)", Tag = "zh-MO"},
            new ComboBoxItem() {Content = "German (Liechtenstein)", Tag = "de-LI"},
            new ComboBoxItem() {Content = "English (New Zealand)", Tag = "en-NZ"},
            new ComboBoxItem() {Content = "Spanish (Costa Rica)", Tag = "es-CR"},
            new ComboBoxItem() {Content = "French (Luxembourg)", Tag = "fr-LU"},
            new ComboBoxItem() {Content = "Arabic (Morocco)", Tag = "ar-MA"},
            new ComboBoxItem() {Content = "English (Ireland)", Tag = "en-IE"},
            new ComboBoxItem() {Content = "Spanish (Panama)", Tag = "es-PA"},
            new ComboBoxItem() {Content = "French (Principality of Monaco)", Tag = "fr-MC"},
            new ComboBoxItem() {Content = "Arabic (Tunisia)", Tag = "ar-TN"},
            new ComboBoxItem() {Content = "English (South Africa)", Tag = "en-ZA"},
            new ComboBoxItem() {Content = "Spanish (Dominican Republic)", Tag = "es-DO"},
            new ComboBoxItem() {Content = "Arabic (Oman)", Tag = "ar-OM"},
            new ComboBoxItem() {Content = "English (Jamaica)", Tag = "en-JM"},
            new ComboBoxItem() {Content = "Spanish (Venezuela)", Tag = "es-VE"},
            new ComboBoxItem() {Content = "Arabic (Yemen)", Tag = "ar-YE"},
            new ComboBoxItem() {Content = "English (Caribbean)", Tag = "en-029"},
            new ComboBoxItem() {Content = "Spanish (Colombia)", Tag = "es-CO"},
            new ComboBoxItem() {Content = "Arabic (Syria)", Tag = "ar-SY"},
            new ComboBoxItem() {Content = "English (Belize)", Tag = "en-BZ"},
            new ComboBoxItem() {Content = "Spanish (Peru)", Tag = "es-PE"},
            new ComboBoxItem() {Content = "Arabic (Jordan)", Tag = "ar-JO"},
            new ComboBoxItem() {Content = "English (Trinidad and Tobago)", Tag = "en-TT"},
            new ComboBoxItem() {Content = "Spanish (Argentina)", Tag = "es-AR"},
            new ComboBoxItem() {Content = "Arabic (Lebanon)", Tag = "ar-LB"},
            new ComboBoxItem() {Content = "English (Zimbabwe)", Tag = "en-ZW"},
            new ComboBoxItem() {Content = "Spanish (Ecuador)", Tag = "es-EC"},
            new ComboBoxItem() {Content = "Arabic (Kuwait)", Tag = "ar-KW"},
            new ComboBoxItem() {Content = "English (Republic of the Philippines)", Tag = "en-PH"},
            new ComboBoxItem() {Content = "Spanish (Chile)", Tag = "es-CL"},
            new ComboBoxItem() {Content = "Arabic (U.A.E.)", Tag = "ar-AE"},
            new ComboBoxItem() {Content = "Spanish (Uruguay)", Tag = "es-UY"},
            new ComboBoxItem() {Content = "Arabic (Bahrain)", Tag = "ar-BH"},
            new ComboBoxItem() {Content = "Spanish (Paraguay)", Tag = "es-PY"},
            new ComboBoxItem() {Content = "Arabic (Qatar)", Tag = "ar-QA"},
            new ComboBoxItem() {Content = "Spanish (Bolivia)", Tag = "es-BO"},
            new ComboBoxItem() {Content = "Spanish (El Salvador)", Tag = "es-SV"},
            new ComboBoxItem() {Content = "Spanish (Honduras)", Tag = "es-HN"},
            new ComboBoxItem() {Content = "Spanish (Nicaragua)", Tag = "es-NI"},
            new ComboBoxItem() {Content = "Spanish (Puerto Rico)", Tag = "es-PR"},
            new ComboBoxItem() {Content = "Sami, Southern (Norway)", Tag = "sma-NO"},
            new ComboBoxItem() {Content = "Serbian (Cyrillic, Bosnia and Herzegovina)", Tag = "sr-Cyrl-BA"},
            new ComboBoxItem() {Content = "Zulu", Tag = "zu-ZA"},
            new ComboBoxItem() {Content = "Xhosa", Tag = "xh-ZA"},
            new ComboBoxItem() {Content = "Frisian (Netherlands)", Tag = "fy-NL"},
            new ComboBoxItem() {Content = "Setswana (South Africa)", Tag = "tn-ZA"},
            new ComboBoxItem() {Content = "Sami, Northern (Sweden)", Tag = "se-SE"},
            new ComboBoxItem() {Content = "Sami, Southern (Sweden)", Tag = "sma-SE"},
            new ComboBoxItem() {Content = "Filipino (Philippines)", Tag = "fil-PH"},
            new ComboBoxItem() {Content = "Sami, Inari (Finland)", Tag = "smn-FI"},
            new ComboBoxItem() {Content = "Quechua (Peru)", Tag = "quz-PE"},
            new ComboBoxItem() {Content = "Sami, Northern (Finland)", Tag = "se-FI"},
            new ComboBoxItem() {Content = "Sami, Skolt (Finland)", Tag = "sms-FI"},
            new ComboBoxItem() {Content = "Welsh", Tag = "cy-GB"},
            new ComboBoxItem() {Content = "Croatian (Bosnia and Herzegovina)", Tag = "hr-BA"},
            new ComboBoxItem() {Content = "Inuktitut (Latin, Canada)", Tag = "iu-Latn-CA"},
            new ComboBoxItem() {Content = "Bosnian (Cyrillic, Bosnia and Herzegovina)", Tag = "bs-Cyrl-BA"},
            new ComboBoxItem() {Content = "Mohawk (Mohawk)", Tag = "moh-CA"},
            new ComboBoxItem() {Content = "Sami, Lule (Norway)", Tag = "smj-NO"},
            new ComboBoxItem() {Content = "Mapudungun (Chile)", Tag = "arn-CL"},
            new ComboBoxItem() {Content = "Maori", Tag = "mi-NZ"},
            new ComboBoxItem() {Content = "Quechua (Ecuador)", Tag = "quz-EC"},
            new ComboBoxItem() {Content = "Irish (Ireland)", Tag = "ga-IE"},
            new ComboBoxItem() {Content = "Romansh (Switzerland)", Tag = "rm-CH"},
            new ComboBoxItem() {Content = "Serbian (Latin, Bosnia and Herzegovina)", Tag = "sr-Latn-BA"},
            new ComboBoxItem() {Content = "Sami, Lule (Sweden)", Tag = "smj-SE"},
            new ComboBoxItem() {Content = "Luxembourgish (Luxembourg)", Tag = "lb-LU"},
            new ComboBoxItem() {Content = "Sesotho sa Leboa (South Africa)", Tag = "ns-ZA"},
            new ComboBoxItem() {Content = "Quechua (Bolivia)", Tag = "quz-BO"},
            new ComboBoxItem() {Content = "Sami, Northern (Norway)", Tag = "se-NO"},
            new ComboBoxItem() {Content = "Maltese", Tag = "mt-MT"},
            new ComboBoxItem() {Content = "Bosnian (Latin, Bosnia and Herzegovina)", Tag = "bs-Latn-BA"}
        };
    }
}
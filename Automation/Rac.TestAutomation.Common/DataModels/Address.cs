using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rac.TestAutomation.Common
{
    public class Address
    {
        private const string DEFAULT_STATE = "WA";
        private const string DEFAULT_COUNTRY = "AUSTRALIA";

        //TODO: This has been left out as managing the variations for units/flats/apartments
        //    has been proving complex to resolve.
        //public int?   UnitNumber    { get; set; }
        [JsonProperty("buildingName")]
        public string buildingName { get; set; }
        [JsonProperty("houseNumber")]
        public string StreetNumber { get; set; }
        [JsonProperty("streetName")]
        public string StreetOrPOBox { get; set; }
        [JsonProperty("blockNumber")]
        public string blockNumber { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("postcode")]
        public string PostCode { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("gnafPid")]
        public string GnafPid { get; set; }
        [JsonProperty("isPreferredDeliveryMethod")]
        public bool IsPreferredDeliveryMethod { get; set; }
        [JsonProperty("isAddressValidated")]
        public bool isAddressValidated { get; set; }
        public string Dpid { get; set; }


        public Address()
        {
            State = DEFAULT_STATE;
            Country = DEFAULT_COUNTRY;
        }

        public Address(string streetNumber = null,
                       string streetName = null,
                       string suburb = null,
                       string postcode = null,
                       string state = null,
                       string country = DEFAULT_COUNTRY)
        {
            StreetNumber = streetNumber;
            StreetOrPOBox = streetName;
            Suburb = suburb;
            PostCode = postcode;
            State = state;
            Country = country;
        }

        /// <summary>
        /// Returns the address in the format of:
        /// {unit#} {street#} {street name}, {suburb} {postcode}
        /// The unit and street numbers are optional and if they 
        /// are not defined, then they are not included.
        /// </summary>
        /// <param name="expandUnitAddresses">if TRUE will expand "A/B street" into "unit A B street"</param>
        public string StreetSuburbPostcode(bool expandUnitAddresses) => $"{NumberAndStreet(expandUnitAddresses)}, {Suburb} {PostCode}";

        /// <summary>
        /// Returns the address in the format of:
        /// {BuildingName#} {street#} {street name}, {suburb} {State} {postcode}
        /// The unit and street numbers are optional and if they 
        /// are not defined, then they are not included.
        /// </summary>
        public string StreetSuburbStatePostcode()
        {
            if (string.IsNullOrEmpty(buildingName) && string.IsNullOrEmpty(State))
            {
                return $"{StreetNumber} {StreetOrPOBox}, {Suburb} {PostCode}";
            }
            else if (string.IsNullOrEmpty(buildingName))
            {
                return $"{StreetNumber} {StreetOrPOBox}, {Suburb} {State} {PostCode}";
            }
            else if (string.IsNullOrEmpty(State))
            {
                return $"{buildingName} {StreetNumber} {StreetOrPOBox}, {Suburb} {PostCode}";
            }
            else
            {
                return $"{buildingName} {StreetNumber} {StreetOrPOBox}, {Suburb} {State} {PostCode}";
            }
        }

        public string StreetSuburbStatePostcode(bool longStreetType) => 
            $"{StreetNumber} {QASStreetNameOnly(longStreetType)}, {Suburb} WA {PostCode}".Trim();        

        /// <summary>
        /// Returns the address in the format of:
        /// {BuildingName#} {street#} {street name}, {suburb} {postcode}
        /// The unit and street numbers are optional and if they 
        /// are not defined, then they are not included.
        /// </summary>
        public string StreetSuburbPostcode()
        {
            return string.IsNullOrEmpty(buildingName) ? $"{StreetNumber} {StreetOrPOBox}, {Suburb} {PostCode}" :
                $"{buildingName} {StreetNumber} {StreetOrPOBox}, {Suburb} {PostCode}";
        }

        public string NumberAndStreet(bool expandUnitAddresses)
        {
            // Convert " st" abbreviations into full "street".
            // Done to support address comparisons. This may need to be added to
            // over time as we find other street types that are getting truncated.

            StreetOrPOBox = ChangeStreetType(StreetOrPOBox?.ToLower() ?? string.Empty, true);

            if (String.IsNullOrWhiteSpace(StreetNumber))
            {
                return StreetOrPOBox;
            }

            if (expandUnitAddresses)
            {
                /* Attempt to provide some uniformity with the way we format addresses
                 * when performing string comparisons, as B2C will format units/apartments
                 * as "Unit <x> <y> <street name>...
                 * But Shield (from QAS) stores it as <x>/<y> street name
                 */
                var regex = new Regex(@"^(\d+)\/(\d+.*)$");
                Match match = regex.Match(StreetNumber.ToLower());
                if (match.Success && match.Groups.Count == 3)
                {
                    return $"unit {match.Groups[1].Value} {match.Groups[2].Value} {StreetOrPOBox}";
                }
            }

            return $"{StreetNumber} {StreetOrPOBox}";
        }

        /// <summary>
        /// Returns the address in the format of:
        /// {unit#}/{street#} {street name}, {suburb} WA
        /// 'WA' is hardcoded as this is assumed for all test scenarios and matches
        /// the majority of our client base. The unit and street numbers are optional
        /// and if they are not defined, then they are not included.
        /// </summary>
        /// <param name="longStreetType">If TRUE, will use long form street type, otherwise will use abbreviated street types.</param>
        /// <returns></returns>
        public string StreetSuburbState(bool longStreetType)
        {
            return string.IsNullOrEmpty(State) ? $"{ChangeStreetType(NumberAndStreet(expandUnitAddresses: true), isExpanding: longStreetType)}, {Suburb}" :
                         $"{ChangeStreetType(NumberAndStreet(expandUnitAddresses: true), isExpanding: longStreetType)}, {Suburb} {State}";
        }

        /// <summary>
        /// Returns the address in the format of:
        /// {unit#}/{street#} {street name}, {suburb} WA
        /// 'WA' is hardcoded as this is assumed for all test scenarios and matches
        /// the majority of our client base. The unit and street numbers are optional
        /// and if they are not defined, then they are not included.
        /// 
        /// Will always use abbreviated street type.
        /// </summary>
        public string StreetSuburbState()
        {
            return string.IsNullOrEmpty(State) ? $"{ChangeStreetType(NumberAndStreet(expandUnitAddresses: true), isExpanding: false)}, {Suburb}" :
                $"{ChangeStreetType(NumberAndStreet(expandUnitAddresses: true), isExpanding: false)}, {Suburb} {State}";
        }

        public string StreetSuburbStateShortened(bool longStreetType)
        {
            return string.IsNullOrEmpty(State) ? $"{ChangeStreetType(NumberAndStreet(expandUnitAddresses: false), longStreetType)}, {Suburb}" :
                 $"{ChangeStreetType(NumberAndStreet(expandUnitAddresses: false), longStreetType)}, {Suburb} {State}";
        }

        /// <summary>
        /// Added to support PPQ flows where address values are pre-populated for
        /// existing contacts with existing QAS verified addresses. In those cases
        /// the street type will already be abbreviated. So we'll want to shorten
        /// our test data version, to match what B2C will be displaying onscreen.
        /// </summary>
        /// <returns></returns>
        public string QASStreetNameOnly(bool longStreetType = false) =>
            ChangeStreetType(this.StreetOrPOBox.ToLower(), longStreetType);

        /// <summary>
        /// Provides full street address, with the abbreviated street type.
        /// </summary>
        /// <param name="longStreetType">If TRUE, expands street type to full string, e.g.: Street, otherwise shortens e.g.: "St"</param>
        /// <returns></returns>
        public string QASStreetAddress(bool longStreetType = false) =>
            $"{StreetNumber} {QASStreetNameOnly(longStreetType)} {Suburb} WA {PostCode}".Trim();

        /// <summary>
        /// Return just the suburb and postcode in format:
        /// {suburb} - {postcode}
        /// </summary>
        /// <returns></returns>
        public string SuburbAndCode() => $"{Suburb} - {PostCode}";

        /// <summary>
        /// Supports formatting the address to align with the string
        /// presented in the PCM portfolio summary view.
        /// </summary>
        /// <returns></returns>
        public string PCMFormattedAddressString() => $"{StreetNumber} {QASStreetNameOnly()} {Suburb}".Trim();

        /// <summary>
        /// For where the UI has presented the address as a string, this 
        /// method is to provide a test to check compare this object
        /// against that string but checking several different formats 
        /// with both abbreviated and full street types, as we don't
        /// know what variant our source and targeted address has been
        /// provided in.
        /// </summary>
        /// <param name="addressString">The string as presented in the B2C UI.</param>
        /// <returns>TRUE or FALSE to reflect if a match was found.</returns>
        public bool IsEqualToString(string addressString)
        {
            if (string.IsNullOrWhiteSpace(addressString)) return false;
            var flattenedAddressString = addressString.StripAddressDelimiters();

            return flattenedAddressString.Equals(QASStreetAddress(longStreetType: false).StripAddressDelimiters(),
                                                 StringComparison.InvariantCultureIgnoreCase) ||
                   flattenedAddressString.Equals(QASStreetAddress(longStreetType: true).StripAddressDelimiters(),
                                                 StringComparison.InvariantCultureIgnoreCase) ||
                   flattenedAddressString.Equals(StreetSuburbState(longStreetType: false).StripAddressDelimiters(),
                                                 StringComparison.InvariantCultureIgnoreCase) ||
                   flattenedAddressString.Equals(StreetSuburbState(longStreetType: true).StripAddressDelimiters(),
                                                 StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsEqual(Address otherAddress) =>
            IsEqualIgnorePostcode(otherAddress) && string.Equals(PostCode, otherAddress.PostCode);

        public bool IsEqualIgnorePostcode(Address otherAddress)
        {
            var isSame = string.IsNullOrEmpty(StreetNumber) ? string.IsNullOrEmpty(otherAddress.StreetNumber) : StreetNumber.Equals(otherAddress.StreetNumber, StringComparison.InvariantCultureIgnoreCase);
            // testing street name with full street type and abbreviated version. As we don't know
            // whether we got it abbreviated from Shield DB.
            isSame &= string.Equals(StreetOrPOBox.ToLower(), otherAddress.QASStreetNameOnly(false)) ||
                      string.Equals(StreetOrPOBox.ToLower(), otherAddress.QASStreetNameOnly(true));
            isSame &= string.Equals(Suburb, otherAddress.Suburb, StringComparison.InvariantCultureIgnoreCase);

            return isSame;
        }

        public static Address ParseString(string textAddress)
        {
            var parsedAddress = new Address();
            string addressNumber = null;

            textAddress = textAddress.ToLower();
            // Append state of WA if not given.
            if (!textAddress.EndsWith(" wa")) { textAddress = textAddress + " wa"; }

            if (textAddress.ToLower().Contains("po box"))
            {
                Regex regexSuburb = new Regex(@"^(?<street>[\w\s\.\/]+(?<number>[0-9\/]*\s)*)\s*,(?<suburb>[A-Za-z'\s]+)(?<postcode>\d+\s)*wa$");
                Match match = regexSuburb.Match(textAddress);
                if (match.Success)
                {
                    parsedAddress.StreetOrPOBox = match.Groups["street"].Value.Trim();
                    parsedAddress.Suburb = match.Groups["suburb"].Value.Trim();
                    parsedAddress.PostCode = match.Groups["postcode"].Value.Trim();
                }
                else
                { Reporting.Error("Could not parse the provided address string of:" + textAddress); }
            }
            else
            {
                Regex regexSuburb = new Regex(@"^(?<number>([A-Za-z]+\s)*[0-9]+[\/\w]*)*(?<street>[\w\s\.\/]+),\s*(?<suburb>[A-Za-z'\s]+)\s(?<postcode>\d+)*\s*wa$");
                Match match = regexSuburb.Match(textAddress);
                if (match.Success)
                {
                    addressNumber = match.Groups["number"].Value.Trim();
                    parsedAddress.StreetOrPOBox = match.Groups["street"].Value.Trim();
                    parsedAddress.Suburb = match.Groups["suburb"].Value.Trim();
                    parsedAddress.PostCode = match.Groups["postcode"].Value.Trim();
                }
                else
                { Reporting.Error("Could not parse the provided address string of:" + textAddress); }
            }
            parsedAddress.StreetNumber = addressNumber;

            return parsedAddress;
        }

        private string ChangeStreetType(string streetName, bool isExpanding)
        {
            var modifiedStreetName = "";

            var streetTypeDictionary = new Dictionary<string, string>()
            {
                {"ally",  "alley"},
                {"ambl",  "amble"},
                {"app",   "approach"},
                {"arc",   "arcade"},
                {"ave",   "avenue"},
                {"ave e", "avenue east"},
                {"ave n", "avenue north"},
                {"ave s", "avenue south"},
                {"ave w", "avenue west"},
                {"bvd",   "boulevard"},
                {"ch",    "chase"},
                {"cir",   "circle"},
                {"cct",   "circuit"},
                {"crcs",  "circus" },
                {"cl",    "close"},
                {"ct",    "court"},
                {"ct e",  "court east"},
                {"ct n",  "court north"},
                {"ct s",  "court south"},
                {"ct w",  "court west"},
                {"ctyd",  "courtyard"},
                {"cres",  "crescent"},
                {"cres n","crescent north"},
                {"crst",  "crest"},
                {"crss",  "cross"},
                {"crss w","cross west"},
                {"crsg",  "crossing"},
                {"crd",   "crossroad"},
                {"cowy",  "crossway"},
                {"cds",   "cul-de-sac"},
                {"dr",    "drive"},
                {"dr e",  "drive east"},
                {"dr n",  "drive north"},
                {"dr s",  "drive south"},
                {"dr w",  "drive west"},
                {"ent",   "entrance"},
                {"esp",   "esplanade"},
                {"gdns",  "gardens"},
                {"gld",   "glade"},
                {"gra",   "grange"},
                {"gr",    "grove"},
                {"hts",   "heights"},
                {"hwy",   "highway"},
                {"ln",    "lane"},
                {"mndr",  "meander"},
                {"pde",   "parade"},
                {"pl",    "place"},
                {"pl n",  "place north"},
                {"prom",  "promenade"},
                {"pkwy",  "parkway"},
                {"rmbl",  "ramble"},
                {"rtt",   "retreat"},
                {"rd",    "road"},
                {"rd e",  "road east"},
                {"rd n",  "road north"},
                {"rd s",  "road south"},
                {"rd w",  "road west"},
                {"sq",    "square"},
                {"sq e",  "square east"},
                {"sq n",  "square north"},
                {"sq s",  "square south"},
                {"sq w",  "square west"},
                {"st",    "street"},
                {"st e",  "street east"},
                {"st n",  "street north"},
                {"st s",  "street south"},
                {"st w",  "street west"},
                {"tce",   "terrace"},
                {"trl",   "trail"},
                {"vsta",  "vista"},
                {"way e", "way east"},
                {"way n", "way north"},
                {"way s", "way south"},
                {"way w", "way west"}
            };

            var streetPieces = streetName.Split(' ');
            if (streetPieces.Length > 1)
            {
                for (int i = 0; i < streetPieces.Length - 1; i++)
                {
                    modifiedStreetName = $"{modifiedStreetName} {streetPieces[i]}";
                }
                modifiedStreetName = modifiedStreetName.TrimStart();

                var streetType = streetPieces[streetPieces.Length - 1];

                if (isExpanding)
                {
                    modifiedStreetName = streetTypeDictionary.ContainsKey(streetType) ?
                                         $"{modifiedStreetName} {streetTypeDictionary[streetType]}" :
                                         $"{modifiedStreetName} {streetType}";
                }
                else
                {
                    modifiedStreetName = streetTypeDictionary.ContainsValue(streetType) ?
                                         $"{modifiedStreetName} {streetTypeDictionary.First(x => x.Value == streetType).Key}" :
                                         $"{modifiedStreetName} {streetType}";
                }
            }
            else
                modifiedStreetName = streetName;

            return modifiedStreetName;
        }
    }
}
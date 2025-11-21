using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.APIDriver;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral.CreditCardIssuer;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.General;
using Rac.TestAutomation.Common.AzureStorage;
using AventStack.ExtentReports.Utils;
using Azure.Data.Tables;
using NUnit.Framework.Internal;
using Rac.TestAutomation.Common.Exceptions;

namespace Rac.TestAutomation.Common
{
    public static class DataHelper
    {
        #region Data Creation
        private static readonly string libraryLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string libraryNumbers = "0123456789";
        private static readonly string libraryAllowableNamePunctuation = "-'() ";
        private static readonly string libraryAllowableEmailPunctuation = @"-_.@";

        private static readonly string[] creditCardNumbersAmex =  { "340000000636513", "370000000201048" } ;
        private static readonly string[] creditCardNumbersMastercard = { "5163200000000008", "5200000009915957", "2224000000031118" };
        private static readonly string[] creditCardNumbersVisa = {"4242424242424242", "4111111111111111", "4444333322221111", "4041370000456459" };

        private static readonly string creditCardNumbersAmexNotSufficientFunds = "340000000089796";
        private static readonly string creditCardNumbersMastercardNotSufficientFunds = "5100000000432896";
        private static readonly string creditCardNumbersVisaNotSufficientFunds = "4111111111444496";

        /// <summary>
        /// Returns a random integer between the provide bounds.
        /// NOTE: random number range includes minValue but excludes maxValue.
        /// </summary>
        /// <returns>A integer that can be equal to minValue to up to (but less than) maxValue</returns>
        public static int RandomNumber(int minValue, int maxValue)
        {
            return Randomiser.Get.Next(minValue, maxValue);
        }

        /// <summary>
        /// Return random boolean value
        /// </summary>
        public static bool RandomBoolean()
        {
            return Randomiser.Get.Next(2) == 1;
        }        

        /// <summary>
        /// Random alphabetical characters, including randomised casing.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomLetters(int length)
        {
            return RandomString(libraryLetters + libraryLetters.ToLower(), length);
        }

        /// <summary>
        /// Random alphabetical characters, including randomised casing.
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength">upper bound, not inclusive</param>
        /// <returns></returns>
        public static string RandomLetters(int minLength, int maxLength)
        {
            int length = Randomiser.Get.Next(minLength, maxLength);
            return RandomString(libraryLetters + libraryLetters.ToLower(), length);
        }

        /// <summary>
        /// Random string that includes allowable punctuation by Shield. Shield
        /// does have a larger list of whitelisted characters (see:
        /// https://rac-wa.atlassian.net/wiki/spaces/SS/pages/885166284/Whitelisted+Characters)
        /// but this method is just adding the values permitted in names and
        /// general text fields by the Spark and B2C applications as well.
        /// </summary>
        public static string RandomShieldWhiteListedCharacters(int minLength, int maxLength)
        {
            int length = Randomiser.Get.Next(minLength, maxLength);
            return RandomString(libraryLetters + libraryLetters.ToLower() + libraryAllowableNamePunctuation, length).Trim();
        }

        /// <summary>
        /// Random string of letters and numbers.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomAlphanumerics(int length)
        {
            return RandomString(libraryLetters + libraryNumbers, length);
        }

        /// <summary>
        /// Random string of letters and numbers.
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength">upper bound, not inclusive</param>
        /// <returns></returns>
        public static string RandomAlphanumerics(int minLength, int maxLength)
        {
            int length = Randomiser.Get.Next(minLength, maxLength);
            return RandomString(libraryLetters + libraryNumbers, length);
        }

        /// <summary>
        /// Random numbers returned in a string 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomNumbersAsString(int length)
        {
            return RandomString(libraryNumbers, length);
        }

        /// <summary>
        /// Random numbers returned in a string 
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength">upper bound, not inclusive</param>
        /// <returns></returns>
        public static string RandomNumbersAsString(int minLength, int maxLength)
        {
            int length = Randomiser.Get.Next(minLength, maxLength);
            return RandomString(libraryNumbers, length);
        }

        private static string RandomString(string chars, int length)
        {
            var stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[Randomiser.Get.Next(chars.Length)];
            return new string(stringChars);
        }

        /// <summary>
        /// Returns a date of birth for a person of 'ageYears' old, where the day of the
        /// year is set randomly during the year, but still come out to that year of
        /// age.
        /// </summary>
        /// <param name="ageYears"></param>
        /// <returns></returns>
        public static DateTime RandomDoB(int ageYears)
        {
            int dayOffset = Randomiser.Get.Next(365);
            var dob = DateTime.Now.AddYears(-ageYears).AddDays(-dayOffset);

            return dob;
        }

        /// <summary>
        /// Provide a random date of birth for contact whose age is inclusive of the years
        /// provided. i.e. if min=10 and max=20, then DoB will range from (10years 0 days old to 20years 364 days old).
        /// </summary>
        /// <param name="minAge">minimum age requested</param>
        /// <param name="maxAge">max age, inclusive.</param>
        /// <returns></returns>
        public static DateTime RandomDoB(int minAge, int maxAge)
        {
            if (minAge >= maxAge)
            {
                Reporting.Error($"Invalid inputs to RandomDoB, minAge: {minAge}, maxAge:{maxAge}) helper.");
            }

            double dayOffset = Randomiser.Get.Next(365 * (maxAge - minAge));
            var dob = DateTime.Now.AddYears(-minAge).AddDays(-dayOffset);

            return dob;
        }

        /// <summary>
        /// Provide a random date from the past X years. Useful for
        /// disclosure of random date in a set time frame, etc.
        /// </summary>
        /// <param name="yearsToGoBack">max age, inclusive.</param>
        /// <returns></returns>
        public static DateTime RandomDateInPastXYears(int yearsToGoBack)
        {
            // We can leverage the RandomDOB method, where we go with a minimum age of 0
            // but with a maximum age being 1 under the years we want to go back.
            // This is because if you want to have at date in the past 3 years, you
            // essentially want something that is UNDER 3 YEARS OLD, not 3yrs and 11mths.
            return RandomDoB(0, yearsToGoBack - 1);
        }

        /// <summary>
        /// Intended to support RACI claims scenarios where we want a random time of day.
        /// Minutes are in increments of 15min (0, 15, 30, 45).
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RandomClaimTimeOfDay(this DateTime dateTime)
        {
            int minutes = Randomiser.Get.Next(0, 4) * 15;  // Give us a random 0,15,30,45 min value.
            int hours   = Randomiser.Get.Next(0, 24);      // 24 hour time from 0:xx to 23:xx

            return dateTime.Date.AddHours(hours).AddMinutes(minutes);
        }

        /// <summary>
        /// Return a date in dd/MM/yyyy format afer adding certain number of days to a date
        /// </summary>
        /// <param name="dateTime">date to which days need to be added</param>
        /// <param name="daysToAdd">number of days to be added</param>
        public static string AddDaysToDate(DateTime dateTime, int daysToAdd)
        {
            return dateTime.Date.AddDays(daysToAdd).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); ;
        }

        /// <summary>
        /// Returns a random Australian mobile number
        /// </summary>
        /// <returns>random mobile number</returns>
        public static string RandomMobileNumber()
        {
            var firstTwoDigits = DataHelper.RandomNumbersAsString(2);
            var secondDigits = DataHelper.RandomNumbersAsString(3);
            var thirdDigits = DataHelper.RandomNumbersAsString(3);

            return $"04{firstTwoDigits}{secondDigits}{thirdDigits}";
        }
        /// <summary>
        /// Returns a new Email object derived from the provided first
        /// and last name. If either of those values is null, then
        /// a random string will be generated in its place.
        /// </summary>
        /// <returns>Email object with new random email.</returns>
        public static Email RandomEmail(string firstName = null, string surname = null, string domain = null)
        {
            firstName = firstName ?? RandomLetters(6);
            surname   = surname ?? RandomLetters(6);
            domain    = string.IsNullOrEmpty(domain) ? NPE_EMAIL_DOMAIN_RACTEST : domain;
            return new Email($@"{new string(firstName.Where(char.IsLetter).ToArray())}.{new string(surname.Where(char.IsLetter).ToArray())}@{domain}");
        }

        /// <summary>
        /// Generates a new email address as a string derived from the provided first and last name.
        /// If either of these values are somehow null a random string of 6 letters will be substituted.
        /// 
        /// This is NOT the same as the private/work contact email addresses in, I expect it will be used 
        /// for the "MyRAC Reg Email" in Member Central.
        /// </summary>
        /// <param name="firstName">First name of the member to be used in generating a login email</param>
        /// <param name="surname">Surname of the member to be used in generating a login email</param>
        /// <param name="domain">Strongly suggest you should be passing through "_config.Email?.Domain"</param>
        /// <returns>Email address string matching the desired domain</returns>
        public static string LoginEmail(string firstName = null, string surname = null, string domain = null)
        {
            firstName = firstName ?? RandomLetters(6);
            surname = surname ?? RandomLetters(6);
            domain = string.IsNullOrEmpty(domain) ? NPE_EMAIL_DOMAIN_RACTEST : domain;
            string emailString = ($@"{new string(firstName.Where(char.IsLetter).ToArray())}.{new string(surname.Where(char.IsLetter).ToArray())}@{domain}");
            return emailString;
        }

        /// <summary>
        /// Random valid credit card for use against Westpac test portal.
        /// Card numbers pulled from https://quickstream.westpac.com.au/docs/general/test-account-numbers/#australian-merchants.
        /// </summary>
        /// <returns>CreditCard using number from Westpac test portal </returns>
        /// <param name="isNotSufficientFundCard">Optional Parameter: pass true value to return credit card with insufficent funds</param>
        /// <exception cref="NotSupportedException">Thrown if credit card issuer is not supported for B2C/Spark apps.</exception>
        public static CreditCard RandomCreditCard(bool isNotSufficientFundCard = false)
        {
            var cc = new CreditCard();
            cc.CardIssuer = GetRandomEnum<Constants.PolicyGeneral.CreditCardIssuer>();
            switch (cc.CardIssuer)
            {
                case Amex:
                    cc.CardNumber = isNotSufficientFundCard ? creditCardNumbersAmexNotSufficientFunds : creditCardNumbersAmex[Randomiser.Get.Next(creditCardNumbersAmex.Length)];
                    cc.CVNNumber = DataHelper.RandomNumbersAsString(4);
                    break;
                case Mastercard:
                    cc.CardNumber = isNotSufficientFundCard ? creditCardNumbersMastercardNotSufficientFunds : creditCardNumbersMastercard[Randomiser.Get.Next(creditCardNumbersMastercard.Length)];
                    cc.CVNNumber = DataHelper.RandomNumbersAsString(3);
                    break;
                case Visa:
                    cc.CardNumber = isNotSufficientFundCard ? creditCardNumbersVisaNotSufficientFunds : creditCardNumbersVisa[Randomiser.Get.Next(creditCardNumbersVisa.Length)];
                    cc.CVNNumber = DataHelper.RandomNumbersAsString(3);
                    break;
                default:
                    throw new NotSupportedException($"Not supported credit card issuer: {cc.CardIssuer}");
            }

            cc.CardholderName = $"{DataHelper.RandomLetters(6)} {DataHelper.RandomLetters(6)}";

            cc.CardExpiryDate = DateTime.Now.AddMonths(DataHelper.RandomNumber(1, 24));

            return cc;
        }

        public static T PickRandom<T>(this IEnumerable<T> list)
        {
            T result;
            var enumerated = list?.ToArray();
            if (enumerated != null && enumerated.Any())
            {
                result = enumerated.Length > 1 ? enumerated[Randomiser.Get.Next(0, enumerated.Length - 1)] : enumerated.Single();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(list), "The list provided has no items to select from");
            }
            return result;
        }

        public static List<T> PickRandom<T>(this IEnumerable<T> list, int count)
        {
            var workingList = list?.ToList() ?? new List<T>();
            var result = new List<T>();

            if (workingList.Count > count)
            {
                for (var i = 0; i < count; i++)
                {
                    result.Add(workingList.PickRandom());
                    workingList.Remove(result[i]);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unable to select a random subset of {count} item(s). List contains only {workingList.Count} item(s)");
            }

            return result;
        }

        public static Vehicle GetRandomInsurableMotorcycle(int minInsurableAmount = 500, int maxInsurableAmount = 200000)
        {
            return Task.Run(() => GetRandomInsurableVehicle(Constants.PolicyGeneral.Vehicle.Motorcycle, minInsurableAmount, maxInsurableAmount)).GetAwaiter().GetResult();
        }

        public static Vehicle GetRandomInsurableCaravan(int minInsurableAmount = CARAVAN_MIN_SUM_INSURED_VALUE, int maxInsurableAmount = CARAVAN_MAX_SUM_INSURED_VALUE)
        {
            return Task.Run(() => GetRandomInsurableVehicle(Constants.PolicyGeneral.Vehicle.Caravan, minInsurableAmount, maxInsurableAmount)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Finds a random motor vehicle, constrained only by price (and excluding invalid rating codes).
        /// Return type is converted to "Car" object, as we also transform the body string.
        /// </summary>
        /// <param name="minInsurableAmount">Minimum desired insurable value (optional)</param>
        /// <param name="maxInsurableAmount">Minimum desired insurable value (optional)</param>
        public static Car GetRandomInsurableCar(int minInsurableAmount = 500, int maxInsurableAmount = 200000)
        {
            var vehicleChosen = Task.Run(() => GetRandomInsurableVehicle(Constants.PolicyGeneral.Vehicle.Car, minInsurableAmount, maxInsurableAmount)).GetAwaiter().GetResult();

            // Shield stores the body of cars with the door count, e.g. "3D Hatchback", "5D Wagon",
            // but B2C and Spark strip that from the dropdowns. So we need to do that too.
            var body = Regex.Replace(vehicleChosen.VehicleSubTypeDescription, "\\d+D\\s", "");

            return new Car()
            {
                Make         = vehicleChosen.MakeDescription,
                Year         = vehicleChosen.ModelYear,
                Model        = vehicleChosen.ModelFamily,
                Body         = body,
                Transmission = vehicleChosen.TransmissionDescription,
                VehicleId    = vehicleChosen.VehicleId,
                MarketValue  = vehicleChosen.Price
            };
        }

        private static async Task<Vehicle> GetRandomInsurableVehicle(Constants.PolicyGeneral.Vehicle vehicleType, int minInsurableAmount = 500, int maxInsurableAmount = 200000)
        {
            var iterationCounter = 0;
            Vehicle vehicle = null;
            do
            {
                iterationCounter++;

                var shieldAPIHandler = new ShieldAPI();
                var manufacturerList = await shieldAPIHandler.GET_SupportedManufacturersAndYearsAsync(vehicleType, minInsurableAmount, maxInsurableAmount);

                if (manufacturerList?.Manufacturers?.Any() == false)
                {
                    // If this occurs, then there's a serious problem. No point retrying.
                    throw new NUnitException("Shield did not provide any manufacturers for our query.");
                }

                var randomManufacturer = manufacturerList.Manufacturers.OrderBy(o => Guid.NewGuid()).First();
                if (randomManufacturer?.Years?.Any() == false)
                {
                    var extCode     = randomManufacturer.ExternalCode ?? "none";
                    var description = randomManufacturer.Description  ?? "none";
                    Reporting.LogAsyncTask($"Manufacturer {extCode}/{description} was returned with no valid year options.");
                    continue;
                }

                var randomYear = randomManufacturer.Years.OrderBy(y => Guid.NewGuid()).First();

                var modelList = await shieldAPIHandler.GET_InsurableModelsForManufacturerAndYear(vehicleType, randomManufacturer.ExternalCode, randomYear, minInsurableAmount, maxInsurableAmount);
                if (modelList.Vehicles?.Any() == false)
                {
                    var extCode     = randomManufacturer.ExternalCode ?? "none";
                    var description = randomManufacturer.Description  ?? "none";
                    Reporting.LogAsyncTask($"Combination of {extCode}/{description} and year:{randomYear} returned no models.");
                    continue;
                }

                vehicle = modelList.Vehicles.OrderBy(m => Guid.NewGuid()).First();

                if (vehicleType == Constants.PolicyGeneral.Vehicle.Motorcycle && string.IsNullOrEmpty(vehicle.ModelFamily))
                {
                    // Some motorcycles have this null. We can't use those.
                    var extCode     = randomManufacturer.ExternalCode ?? "none";
                    var description = randomManufacturer.Description  ?? "none";

                    Reporting.LogAsyncTask($"Combination of {extCode}/{description}, year:{randomYear}, model:{vehicle.ModelDescription} returned a null-empty engine capacity.");
                    continue;
                }

                if (vehicle.ModelDescription.Contains("'") ||
                    vehicle.ModelDescription.StartsWith(" ") ||
                    vehicle.ModelDescription.EndsWith(" "))
                {
                    // If there are apostrophes in vehicle model, it breaks XPath logic, so we can't use.
                    // If the model starts or ends with a whitespace, we also can't use. See ISE-10447.
                    // Once ISE-10447 is fixed, the whitespace checks can be removed.
                    var extCode     = randomManufacturer.ExternalCode ?? "none";
                    var description = randomManufacturer.Description  ?? "none";

                    Reporting.LogAsyncTask($"Combination of {extCode}/{description}, year:{randomYear}, model:{vehicle.ModelDescription} returned a model name we can't support.");
                    continue;
                }

                break; // Gets us out of the loop. "vehicle != null" will show that we had success.
            } while (iterationCounter <= 3);

            if (vehicle == null)
            { throw new NUnitException("Attempted 3 times to find a valid random vehicle via Shield API and failed."); }

            return vehicle;
        }

        /// <summary>
        /// Searches for a low value vehicle (under $40k) with a low risk rating.
        /// This is primarily intended to motor endorsement tests where we are
        /// changing to one of these vehicles in order to trigger a decrease in 
        /// premium and possible refund.
        /// 
        /// We are searching manufacturers whose vehicles typically have a low
        /// insurance risk rating, as well as low market value offerings.
        /// </summary>
        public static Car FindRandomLowValueLowRatingVehicle()
        {
            var lowValueManufacturers = new List<string>
            { Manufacturers.Ford, Manufacturers.Holden, Manufacturers.Mazda,
              Manufacturers.Mitsubishi, Manufacturers.Nissan, Manufacturers.Toyota };

            return Task.Run(() => FindARandomVehicleWithinPriceAndRatingRange(lowValueManufacturers, minValue: 10000, maxValue: 40000, minRating:0, maxRating: 40)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Searches for a high value vehicle which also has a high risk rating
        /// (risk rating and value are not directly related). This is for motor
        /// endorsement tests where we wish to trigger an increase in premium.
        /// 
        /// We are searching only among manufacturers who typically have a higher
        /// insurance risk rating.
        /// </summary>
        public static Car FindRandomHighValueHighRatingVehicle()
        {
            var highValueManufacturers = new List<string>
            { Manufacturers.Audi, Manufacturers.BMW, Manufacturers.Citroen,
              Manufacturers.Mercedes, Manufacturers.Porsche };

            return Task.Run(() => FindARandomVehicleWithinPriceAndRatingRange(highValueManufacturers, minValue: 50000, maxValue: 140000, minRating: 40, maxRating: 100)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Only for motor vehicles.
        /// 
        /// Find a random vehicle within the pricing range and rating range given, and for the
        /// specified manufacturers.
        /// </summary>
        /// <param name="manufacturerCodes">List of 3 letter manufacturer codes to include in search.</param>
        /// <param name="minValue">Minimum market value desired</param>
        /// <param name="maxValue">Maximum market value desired</param>
        /// <param name="minRating">Minimum desired risk rating.</param>
        /// <param name="maxRating">Maximum desired risk rating.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private static async Task<Car> FindARandomVehicleWithinPriceAndRatingRange(List<string> manufacturerCodes, int minValue, int maxValue, int minRating, int maxRating)
        {
            var vehicleCandidates = new List<Vehicle>();
            Vehicle vehicleChosen = null;

            var shieldAPIHandler = new ShieldAPI();

            foreach(var manufacturerCode in manufacturerCodes )
            {
                var vehicleList = await shieldAPIHandler.GET_InsurableModelsForManufacturerAndYear(vehicleType: Constants.PolicyGeneral.Vehicle.Car,
                                                                                   manufacturerCode: manufacturerCode,
                                                                                   year: 0,
                                                                                   minimumValue: minValue,
                                                                                   maximumValue: maxValue);

                var thing = vehicleList.Vehicles.Where(x => x.RatingPoints >= minRating && x.RatingPoints <= maxRating).ToList();
                vehicleCandidates = vehicleCandidates.Concat(thing).ToList();
            }

            vehicleChosen = vehicleCandidates.OrderBy(y => Guid.NewGuid()).FirstOrDefault();

            if (vehicleChosen == null)
                throw new InvalidDataException($"Unable to find a random vehicle of rating between {minRating} and {maxRating}");

            // Shield stores the body of cars with the door count, e.g. "3D Hatchback", "5D Wagon",
            // but B2C and Spark strip that from the dropdowns. So we need to do that too.
            var body = Regex.Replace(vehicleChosen.VehicleSubTypeDescription, "\\d+D\\s", "");

            return new Car()
            {
                Make         = vehicleChosen.MakeDescription,
                Year         = vehicleChosen.ModelYear,
                Model        = vehicleChosen.ModelFamily,
                Body         = body,
                Transmission = vehicleChosen.TransmissionDescription,
                VehicleId    = vehicleChosen.VehicleId,
                MarketValue  = vehicleChosen.Price
            };
        }

        /// <summary>
        /// Returns a random payment scenario from payment scenario/s given
        /// </summary>
        /// <param name="paymentScenarios"></param>
        /// <returns></returns>
        public static Constants.PolicyGeneral.PaymentScenario GetRandomPaymentScenario(params Constants.PolicyGeneral.PaymentScenario[] paymentScenarios) => paymentScenarios[Randomiser.Get.Next(0, paymentScenarios.Length)];

        /// <summary>
        /// Creates a policyHolder contact with the given:
        /// 1. matchRule: Member match Rule (e.g:-MemberMatchRule.Rule1)
        /// 2. declareMembership:   true:   if member is selecting "Yes" for the "Are you an RAC member question"
        ///                         false:  if memeber is selecting "No" for the "Are you an RAC member question"
        /// 3. membershipTiers: One or more tiers: Gold/Silver/Bronze etc.. (e.g:- new[] {MembershipTier.Gold, MembershipTier.Silver, MembershipTier.Bronze})                         
        /// </summary>
        /// <param name="testConfig"></param>
        /// <param name="matchRule">Member matching rule that is to be assessed (causes Contact data to be modified to ensure only that rule will pass)</param>
        /// <param name="declareExistingMembership">Sets associated flag in Contact object</param>
        /// <param name="minimumAge">inclusive, minimum age required for contact</param>
        /// <param name="maximumAge">inclusive, maximum age required for contact</param>
        /// <param name="gender">if null any gender, otherwise required gender for contact</param>
        /// <param name="membershipTiers">required membership tier for contact's current RSA</param>
        /// <returns></returns>
        public static Contact CreateMemberMatchContact(MemberMatchRule matchRule = MemberMatchRule.None,
                                                       bool declareExistingMembership = true,
                                                       int minimumAge = MIN_PH_AGE_HOME_PET,
                                                       int maximumAge = MAX_PH_AGE,
                                                       Gender? gender = null,
                                                       params MembershipTier[] membershipTiers)
        {
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(minAge: minimumAge, maxAge: maximumAge, gender: gender, membershipTiers: membershipTiers);
            var policyHolder = new ContactBuilder(contactCandidate)
                .WithoutDeclaringMembership(!declareExistingMembership)
                .WithMemberMatchRule(matchRule)
                .Build();
            return policyHolder;
        }

        /// <summary>
        /// Selects a random title based on the provided gender.
        /// NOTE: "Dr" is excluded because not all B2C flows recognise that title
        /// (e.g.: Witnesses in B2C claims cannot have that title).
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="excludeMxTitle">if TRUE, then that title is excluded</param>
        /// <returns></returns>
        public static Title GetRandomTitleForGender(Gender gender, bool excludeMxTitle = false, bool excludeDrTitle = false)
        {
            var excludedTitles = new List<Title>();
            if (excludeDrTitle) excludedTitles.Add(Title.Dr);
            if (excludeMxTitle) excludedTitles.Add(Title.Mx);

            var femaleOptions = new[]
            {
                Title.Dr,
                Title.Miss,
                Title.Mrs,
                Title.Ms,
                Title.Mx
            };

            var maleOptions = new[]
            {
                Title.Dr,
                Title.Mr,
                Title.Mx
            };

            return gender == Gender.Male ? maleOptions.OrderBy(t => Guid.NewGuid()).First(x => !excludedTitles.Contains(x))
                                         : femaleOptions.OrderBy(t => Guid.NewGuid()).First(x => !excludedTitles.Contains(x));
        }
        #endregion

        #region Enumerator support
        /// <summary>
        /// Takes a gender string and converts it to the automation enumeration.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown if we can't convert the provided string</exception>
        public static Gender ConvertGenderStringToEnum(string genderString)
        {
            if (string.IsNullOrEmpty(genderString))
            { throw new InvalidDataException("Error: Provided gender string was null or empty. Cannot convert to an enum."); }

            Gender gender;
            if (!Enum.TryParse(genderString, out gender))
            { throw new InvalidDataException($"No matching gender key found for {genderString}"); }

            return gender;
        }

        public static MembershipTier ConvertMembershipTierStringToEnum(string tier)
        {
            var enumEquivalent = MembershipTier.None;

            if (string.IsNullOrEmpty(tier))
                return enumEquivalent;

            // Captures "Gold" and "Gold Life"
            if (tier.StartsWith("gold", ignoreCase: true, CultureInfo.InvariantCulture))
                enumEquivalent = MembershipTier.Gold;
            else if (tier.StartsWith("silver", ignoreCase: true, CultureInfo.InvariantCulture))
                enumEquivalent = MembershipTier.Silver;
            else if (tier.StartsWith("bronze", ignoreCase: true, CultureInfo.InvariantCulture))
                enumEquivalent = MembershipTier.Bronze;
            else if (tier.StartsWith("red", ignoreCase: true, CultureInfo.InvariantCulture))
                enumEquivalent = MembershipTier.Red;
            else if (tier.StartsWith("blue", ignoreCase: true, CultureInfo.InvariantCulture))
                enumEquivalent = MembershipTier.Blue;
            else if (tier.StartsWith("free2go", ignoreCase: true, CultureInfo.InvariantCulture))
                enumEquivalent = MembershipTier.Free2Go;

            return enumEquivalent;
        }

        /// <summary>
        /// Takes a system presented string and attempts to convert it
        /// to the corresponding home type enumeration.
        /// </summary>
        /// <param name="buildingType">The string to convert to enum</param>
        /// <param name="isShieldText">if TRUE will check against Shield values, otherwise will use B2C variants.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the value was null</exception>
        public static HomeType ConvertBuildingTypeStringToEnum(string buildingType, bool isShieldText = false)
        {
            if (string.IsNullOrEmpty(buildingType))
            {
                throw new ArgumentException("Error: Building type string was null or empty. Cannot convert to an enum.");
            }

            return isShieldText ?
                   HomeTypeDropdownText.First(x => x.Value.TextShield.Equals(buildingType, StringComparison.InvariantCultureIgnoreCase)).Key :
                   HomeTypeDropdownText.First(x => x.Value.TextB2C.Equals(buildingType, StringComparison.InvariantCultureIgnoreCase)).Key;
        }

        /// <summary>
        /// Get a random value for a given enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="startIndex">Start from enum index</param>
        /// <returns></returns>
        public static T GetRandomEnum<T>(int startIndex = 0)
        {
            Array values = Enum.GetValues(typeof(T));

            return (T)values.GetValue(Randomiser.Get.Next(startIndex, values.Length));
        }

        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (string.Equals(attribute.Description, description, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (string.Equals(field.Name, description, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException($"Unable to match {description} to a recognised enumeration {nameof(T)} value.");
        }
        #endregion

        #region data handling
        /// <summary>
        /// Originally created to support B2C-2707 where certain special
        /// characters should get replaced by B2C from user inputted
        /// values before sending to Shield, to supported variants.
        /// </summary>
        /// <param name="userInputValue"></param>
        /// <returns></returns>
        public static string SanitizeString(this string userInputValue)
        {
            var sanitizedText = userInputValue.Replace("—", "-").Replace("’", "'").Replace("‘", "'").Replace("`", "'");

            Reporting.Log($"SANITIZED NAME FROM {userInputValue} INTO NEW VALUE OF {sanitizedText}");
            return sanitizedText;
        }

        /// <summary>
        /// This method removes invalid characters for a vehicle registration plate.
        /// For vehicle registration number, only alphanumeric characters and internal spaces are accepted.
        /// Historically, some special characters such as hyphens are used in Shield, eg RAC-123 when it should be RAC 123.
        /// This method is to provide a 'cleaned' representation for the supplied string.
        /// </summary>
        public static string SanitizeVehicleRegistrationString(this string stringToClean)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 ]");
            return rgx.Replace(stringToClean, "").Trim();
        }

        /// <summary>
        /// Remove round brackets () from the input string
        /// </summary>        
        public static string RemoveRoundBrackets(this string userInputValue)
        {
            return userInputValue.Replace("(", "").Replace(")", "");            
        }

        public static bool NameStringHasInvalidCharacters(string stringToCheck)
        {
            string allowedChars = $"{libraryLetters}{libraryLetters.ToLower()}{libraryAllowableNamePunctuation}";

            // Null/empty string is invalid by default.
            if (string.IsNullOrEmpty(stringToCheck)) return true;

            bool hasOnlyValidChars = stringToCheck.All(allowedChars.Contains);

            return !hasOnlyValidChars;
        }

        public static bool MobileNumberHasInvalidCharactersOrInvalidlength(string stringToCheck)
        {
            string allowedChars = libraryNumbers;

            // Null/empty string is invalid by default.
            if (string.IsNullOrEmpty(stringToCheck)) return true;

            bool hasOnlyValidChars = stringToCheck.All(allowedChars.Contains) && (stringToCheck.Length == 10);

            return !hasOnlyValidChars;
        }

        /// <summary>
        /// Return nicely formatted mobile phone number
        /// for example if mobile number is 04123456789, then returns 0412 345 678
        /// else for landline number 0812345678, it returns 08 1234 5678
        /// </summary>
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("04"))
            {
                return Regex.Replace(phoneNumber, @"(\d{4})(\d{3})(\d{3})", "$1 $2 $3");
            }
            else
            {
                return Regex.Replace(phoneNumber, @"(\d{2})(\d{4})(\d{4})", "$1 $2 $3");
            }
        }

        /// <summary>
        /// Return masked phone number
        /// Mobile number returns as 04** *** 123
        /// Landline      returns as 08 **** *123
        /// </summary>
        public static string MaskPhoneNumber(string phoneNumber)
        {
            Reporting.IsTrue(phoneNumber.Length == 10, $"phone number ({phoneNumber}) was 10 digits in length");
            var areaCode = phoneNumber.Substring(0, 2);
            var lastDigits = phoneNumber.Substring(phoneNumber.Length - 3, 3);

            return areaCode == PhonePrefix.Mobile.GetDescription() ?
                           $"{areaCode}** *** {lastDigits}" :  // Mobile
                           $"{areaCode} **** *{lastDigits}";   // Landline
        }

        /// <summary>
        /// Return email address with all but first and last characters of prefix masked by *.
        /// For example if mobile number is christopher@columbus.com, then returns c*********r@columbus.com
        /// which should match the masked email address in our spark applications.
        /// If there are only one or 2 characters in the prefix, ALL of the prefix will be  masked 
        /// (e.g. **@columbus.com).
        /// </summary>
        public static string MaskEmailAddress(string emailAddress)
        {
            int atIndex = emailAddress.IndexOf('@');
            if (atIndex == -1 || atIndex == 0 || atIndex == emailAddress.Length - 1)
            {
                return emailAddress;
            }

            var maskedString = new StringBuilder(emailAddress);
            if (atIndex <= 2)
            {
                for (int i = 0; i < atIndex; i++)
                {
                    maskedString[i] = '*';
                }
            }
            else
            {
                for (int i = 1; i < atIndex - 1; i++)
                {
                    maskedString[i] = '*';
                }
            }
            return maskedString.ToString();
        }

        /// <summary>
        /// Check for whether a number matches the pattern for Austrlian phone numbers.
        /// Numbers must include area code.
        /// </summary>
        /// <param name="stringToCheck"></param>
        /// <returns></returns>
        public static bool IsValidAustralianPhoneNumber(string stringToCheck)
        {
            Regex phoneNumberRegex = new Regex(@"^0[23478]\d{8}$");

            return !String.IsNullOrWhiteSpace(stringToCheck) &&
                phoneNumberRegex.IsMatch(stringToCheck);
        }

        /// <summary>
        /// Returns whether the supplied number(s) will be acceptable to the Insurance Contact Service (ICS).
        /// 
        /// For the contact service, it prefers just to use the mobile number. It can use a 
        /// home phone number in lieu of a mobile number.   However, a partial mobile number
        /// ie 123456 and a correct home phone number will still cause an error.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="homePhone"></param>
        /// <returns></returns>
        public static bool HasValidPhoneNumberSet(string mobile = null, string homePhone = null)
        {
            // Valid mobile phone number to use (ICS will not consider the home phone number)
            if (DataHelper.IsValidAustralianPhoneNumber(mobile))
            {
                return true;
            }

            // We can only use the home phone number when it is valid AND mobile phone is null or empty
            // a partial phone number such as 123456 will cause an error
            if (string.IsNullOrEmpty(mobile) && DataHelper.IsValidAustralianPhoneNumber(homePhone))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// RAC has strict rules around valid emails, so we've built
        /// our own simple checks. The .Net Mail
        /// </summary>
        /// <param name="stringToCheck"></param>
        /// <returns></returns>
        public static bool IsEmailInvalidFormat(Email emailToCheck)
        {
            string allowedChars = $"{libraryLetters}{libraryLetters.ToLower()}{libraryAllowableEmailPunctuation}";

            // Null/empty string is invalid by default.
            if (emailToCheck == null ||
                string.IsNullOrEmpty(emailToCheck.Address)) return true;

            bool isValid = emailToCheck.Address.All(allowedChars.Contains) &
                           Regex.IsMatch(emailToCheck.Address, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

            return !isValid;
        }

        /// <summary>
        /// Check whether a registration number is known or considered to have a value pending.
        /// There are a few values in SHIELD that are considered as pending / 'To be provided'.
        /// For these unknown and null values, this function will return false.
        /// </summary>
        /// <param name="registrationToCheck"></param>
        /// <returns></returns>
        public static bool IsRegistrationNumberConsideredValid(string registrationToCheck)
        {
            string[] unknownRegistationNumbers = { "TBC", "TBA", "N/A", "NA" };
            if (string.IsNullOrEmpty(registrationToCheck) || unknownRegistationNumbers.Contains(registrationToCheck.ToUpperInvariant()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Removes the monetary notations used in strings such as
        /// "$" and "," symbols. This is to facilitate simpler string
        /// comparisons, as well as conversions to decimal/int.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StripMoneyNotations(this string text)
        {
            return text.Trim().Replace("$", string.Empty).Replace(",", string.Empty);
        }

        /// <summary>
        /// Removes comma characters from an address string to make
        /// it simpler to perform string comparisons for addresses
        /// from different sources.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StripAddressDelimiters(this string text)
        {
            return text.Trim().Replace(",", string.Empty);
        }

        /// <summary>
        /// Replaces any groups of "\r" or "\n" characters in captured HTML text,
        /// with a simple space character, or optionally just removes them.
        /// This is to ease string comparisons and regex operations on blocks of
        /// formatted text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replaceWithWhiteSpace">Defines whether the newlines are replaced with a whitespace, or just removed</param>
        /// <returns></returns>
        public static string StripLineFeedAndCarriageReturns(this string text, bool replaceWithWhiteSpace = true)
        {
            var replacementText = replaceWithWhiteSpace ? " " : string.Empty;
            return Regex.Replace(text.Trim(), @"[\r|\n]+", replacementText);
        }

        /// <summary>
        /// This helper will remove leading and trailing whitespace from the string, and
        /// will also replace any instances of multiple sequential whitespace with a single
        /// space. This logic treats tab as whitespace also.
        /// </summary>
        public static string RemoveDuplicateWhiteSpaceAndTrim(this string text) => Regex.Replace(text, @"\s+", " ").Trim();

        /// <summary>
        /// Method to convert an integer amount into a corresponding monetary
        /// string which is prefixed by a '$' sign, and has the thousands
        /// comma separator.
        /// </summary>
        /// <param name="amount">integer value to convert</param>
        /// <param name="minValueForThousandsSeparator">the minimum value for thousands separator to apply</param>
        /// <param name="applyThousandsSeparator">flag to turn off thousands separator altogether</param>
        /// <returns></returns>
        public static string ConvertIntToMonetaryString(int amount, int minValueForThousandsSeparator = 1000, bool applyThousandsSeparator = true)
        {
            return (amount >= minValueForThousandsSeparator) && applyThousandsSeparator ?
                              $"${amount.ToString("n0")}" : $"${amount}";
        }

        /// <summary>
        /// Converts a monetary string to an int. if the string contains a decimal, it
        /// truncates that value (essentially rounding down to the nearest dollar).
        /// </summary>
        /// <param name="moneyString"></param>
        /// <returns></returns>
        public static int ConvertMonetaryStringToInt(string moneyString)
        {
            return int.Parse(moneyString.Split('.')[0].StripMoneyNotations());
        }

        /// <summary>
        /// This method returns the monetary part in a given string,
        /// after removing anymonetary notations used in strings such as "$" and "," symbols.
        /// e.g:- Returns "12500" from the string "Insured amount $12,500"
        /// </summary>
        /// <param name="stringToExtractMoneyValue"></param>
        /// <returns></returns>
        public static decimal GetMonetaryValueFromString(string stringToExtractMoneyValue)
        {
            var urlRegex = new Regex(@"\$((\d*,)*\d+(\.\d+)*)$");
            Match match = urlRegex.Match(stringToExtractMoneyValue);
            Reporting.IsTrue(match.Success && match.Groups.Count > 1, $"Unable to parse monetary value from UI text: {stringToExtractMoneyValue}");

            return decimal.Parse(match.Groups[1].Value.StripMoneyNotations());
        }

        /// <summary>
        /// Adds the currency prefix to the amount
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currencyPrefix"></param>
        /// <returns></returns>
        public static string AddCurrencyPrefixToAmount(string amount, string currencyPrefix)
        {
            return amount.StartsWith($"{currencyPrefix}") ? amount : $"{currencyPrefix}{amount}";
        }

        /// <summary>
        /// Splits a given string based on the given character  
        /// to create a string array and returns an specific element from the array
        /// </summary>
        /// <param name="stringToSplit"></param>
        /// <param name="delimiter"></param>
        /// <param name="positionToReturn"></param>
        /// <returns>An specific element from the splitted array</returns>
        public static string SplitStringAndReturnAnElementFromArray(string stringToSplit, char delimiter, int positionToReturn)
        {
            try
            {
                string[] stringsList = stringToSplit.Split(delimiter);
                return stringsList[positionToReturn];
            }
            catch (Exception ex)
            {
                Reporting.Log("Exception occured querying DB: " + ex.Message);
                return null;
            }
        }
		
        /// <summary>
        /// This method Rounds Up/Down, the given value based on the following rules:
        /// 1. Round up to the nearest 100 if the percentage is a negative value
        /// 2. Round down to the nearest 100 if the percentage is a positive value
        /// </summary>
        /// <param name="original"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static int RoundUpOrDownToNearestHundred(int original, int percentage)
        {
            // Divide by 100 and round then *100, to get to desired whole hundred value.
            var adjustedValue = percentage > 0 ? (Math.Floor((decimal)original / 100)) * 100 :
                                                 (Math.Ceiling((decimal)original / 100)) * 100;
            return (int)adjustedValue;
        }

        /// <summary>
        /// Takes a string value that is expected to either be "yes" or
        /// "no" and converts that to a boolean, where yes=true, no=false.
        /// If the value has any other value, then a null is returned.
        /// Mainly intended to deal with strings from Shield
        /// questionnaires.
        /// </summary>
        public static bool? StringYesNoToNullableBool(string text)
        {
            bool? valueBool = null;

            if (!string.IsNullOrEmpty(text))
            {
                if (text.Equals(Yes, StringComparison.InvariantCultureIgnoreCase))
                    valueBool = true;
                if (text.Equals(No, StringComparison.InvariantCultureIgnoreCase))
                    valueBool = false;
            }

            return valueBool;
        }

        /// <summary>
        /// Convert a boolean to a "Yes" or "No" string.
        /// Mainly intended to deal with strings from Shield
        /// questionnaires.
        /// </summary>
        public static string BooleanToStringYesNo(bool booleanValue)
        {
            return booleanValue ? Yes : No;
        }

        /// <summary>
        /// Convert a boolean to a "Yes" or "No" string.
        /// Mainly intended to deal with strings from Shield
        /// questionnaires.
        /// </summary>
        public static string BooleanToStringYesNoAndCustomText(bool? booleanValue, string nullText)
        {
            if(booleanValue == null)
            {
                return nullText;
            }
            else
            {
                return booleanValue == true ? Yes : No;
            }
        }

        /// <summary>
        /// Convert given string to Title case.
        /// </summary>
        public static string ToTitleCase(this string inputString)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(inputString.ToLower());
        }

        /// <summary>
        /// Convert the only first char to upper case for the given string
        /// </summary>
        public static string FirstCharToUpper(this string inputString)
        {
            switch (inputString)
            {
                case null: throw new ArgumentNullException(nameof(inputString));
                case "": throw new ArgumentException($"{nameof(inputString)} cannot be empty", nameof(inputString));
                default: return inputString[0].ToString().ToUpper() + inputString.Substring(1);
            }
        }

        /// <summary>
        /// Check if given double number has decimal in it
        /// </summary>
        public static bool IsWholeNumber(double number)
        {
            return Math.Abs(number % 1) < double.Epsilon;
        }

        /// <summary>
        /// Return a random datetime between a range supplied
        /// </summary>
        public static DateTime GenerateRandomDate(DateTime from, DateTime to)
        {
            var range = to - from;
            var randTimeSpan = new TimeSpan((long)(((double)Randomiser.Get.Next(1000)/(double)1000) * range.Ticks));
            return from + randTimeSpan;
        }

        /// <summary>
        /// Converting the int number to ordinals
        /// i.e. 1 returns 1st, 2 returns 2nd
        /// </summary>
        /// <param name="num"> pass the day of the month</param>
        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
                default:
                    break;
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
        /// <summary>
        /// Check if the string have any special char
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>true if any cpecial char found</returns>
        public static bool HasAnySpecialChar(this string inputString)
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (!string.IsNullOrEmpty(inputString) &&
                regexItem.IsMatch(inputString))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// B2C and Spark applications only support a subset of Titles;
        /// Mr, Miss, Mrs, Ms, Mx and Dr. Returns if the title is valid.
        /// </summary>
        /// <param name="titleAsString">Accepts title as plain text for assessment.</param>
        /// <returns>True if the title is valid for B2C/Spark apps; otherwise, false.</returns>
        public static bool IsValidTitle(string titleAsString)
        {
            Title title;
            try
            { title = DataHelper.GetValueFromDescription<Title>(titleAsString); }
            catch
            { title = Title.None; }

            return IsValidTitle(title);
        }

        /// <summary>
        /// B2C and Spark applications only support a subset of Titles;
        /// Mr, Miss, Mrs, Ms, Mx and Dr. Returns if the title is valid.
        /// </summary>
        /// <returns>True if the title is valid for B2C/Spark apps; otherwise, false.</returns>
        public static bool IsValidTitle(Title title)
        {
            switch (title)
            {
                case Title.Miss:
                case Title.Mr:
                case Title.Mrs:
                case Title.Ms:
                case Title.Mx:
                case Title.Dr:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region API call wrappers
        /// <summary>
        /// Takes a Contact object and sends it to Shield's Contact API
        /// to create as a new contact. No checks are made on mandatory
        /// fields. For Contact fields passed through to Shield, see the
        /// definitions of the API_Contact class.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>Contact ID of the newly created Shield contact</returns>
        public static string CreateContactInShieldViaAPI(Contact contact)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.POST_CreateContact(contact)).GetAwaiter().GetResult();
        }

        public static GetQuotePolicy_Response GetQuoteDetails(string quoteNumber)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_QuoteDetails(quoteNumber)).GetAwaiter().GetResult();
        }

        public static GetQuotePolicy_Response GetPolicyDetails(string policyNumber)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_Policy(policyNumber)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Calls the Shield Contact API to retrieve the contact by the Shield Contact ID.
        /// NOTE: Person ID is not known in Shield and won't be returned in the response.
        /// </summary>
        /// <param name="contactId">Contact ID to be retrieved.</param>
        /// <returns>A populated Contact object, otherwise throws an exception if the
        /// contact cannot be found.</returns>
        public static Contact GetContactDetailsViaContactId(string contactId)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_ContactDetailsViaContactId(contactId)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Calls the Shield Contact API to retrieve the contact by the policy number and Shield Contact ID.
        /// Verify the IsPreferredDeliveryMethod from the response for Main PolicyHolder
        /// Stop the testing and throw error message when it is neither email nor mail
        /// </summary>
        public static PreferredDeliveryMethod GetPreferredDeliveryMethodForMainPolicyholder(string policyNumber)
        {
            var policyDetails = GetPolicyDetails(policyNumber);
            var policyholderId = policyDetails.Policyholder.Id.ToString();
            var contact = GetContactDetailsViaContactId(policyholderId);

            var emailPDM = contact.PrivateEmail?.IsPreferredDeliveryMethod.Value == true;
            var postPDM  = contact.MailingAddress?.IsPreferredDeliveryMethod == true;
            Reporting.IsTrue(emailPDM ^ postPDM, "the preferred delivery method must be either email or mail.");

            return emailPDM ? PreferredDeliveryMethod.Email : PreferredDeliveryMethod.Mail;
        }

        /// <summary>
        /// Fetch the person information from Member Central by using Shield ContactId
        /// </summary>
        public static API_MemberCentralPersonV2Response GetPersonFromMemberCentralByContactId(string contactId)
        {
            return Task.Run(() => MemberCentral.GetInstance().GET_PersonByShieldContactId(contactId)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Fetch the contact information for a Shield Contact based on providing the External_Contact_Number
        /// rather than the Contact ID value.
        /// </summary>
        /// <param name="externalContactNumber"></param>
        /// <returns></returns>
        public static Contact GetContactDetailsViaExternalContactNumber(string externalContactNumber)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_ContactDetailsViaExternalContactNumber(externalContactNumber)).GetAwaiter().GetResult();
        }

        public static GetVehicle_Response GetVehicleDetails(string vehicleId)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_VehicleDetails(vehicleId)).GetAwaiter().GetResult();
        }

        public static List<PolicyDetail> GetAllPoliciesOfTypeForPolicyHolder(string contactId, string policyType)
        {
            var linkedPolicies = new List<PolicyDetail>();
            var mcRecord = GetPersonFromMemberCentralByContactId(contactId);
            var memberContactIds = mcRecord.GetAllLinkedShieldIds();

            var portfolioSummary = GetPortfolioSummary(memberContactIds);
            return portfolioSummary.contacts.Where(c => c.policyDetails != null).SelectMany(c => c.policyDetails).Where(x => x.policyType.description == policyType).ToList();
        }

        public static GetPortfolioSummary_Response GetPortfolioSummary(List<string> contactIds)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_PortfolioSummary(contactIds)).GetAwaiter().GetResult();
        }

        public static GetFenceSettlementBreakdownCost_Response GetFenceSettlementBreakdownCost(string claimNumber)
        {
            var shieldAPIHandler = new ShieldAPI();
            try
            {
                return Task.Run(() => shieldAPIHandler.GET_FenceSettlementBreakdown(claimNumber)).GetAwaiter().GetResult();
            }
            catch (ShieldApiException ex)
            {
                Reporting.Error($"Unable to fetch the fence settlement for claim: {claimNumber}- {ex}");
                return null;
            }
        }

        public static GetClaimResponse GetClaimDetails(string claimNumber)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_ClaimDetails(claimNumber)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the service provider details from Shield 
        /// for the claim number and preferred repairer suburb
        /// </summary>
        /// <param name="claimNumber">claim number is required</param>
        /// <param name="suburb">Preferred repairer suburb is required</param>
        /// <returns></returns>
        public static SearchServiceProviders GetServiceProviders(string claimNumber, string suburb)
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.POST_SearchServiceProviders(claimNumber, suburb)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the list of insurance companies from Shield
        /// </summary>
        public static List<API_InsuranceCompanies> GetInsuranceCompany()
        {
            var shieldAPIHandler = new ShieldAPI();

            return Task.Run(() => shieldAPIHandler.GET_InsuranceCompanies()).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Check existing bank account name has any special char
        /// </summary>
        /// <param name="contactID"></param>
        /// <returns>true if any special char found.</returns>
        public static bool ContactHasBadBankAccountName(string contactID)
        {
            var contactDetails = DataHelper.GetContactDetailsViaContactId(contactID);

            foreach (var bankAccount in contactDetails.BankAccounts ?? Enumerable.Empty<BankAccount>())
            {
                if (bankAccount.AccountName.HasAnySpecialChar())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        // It will call the both claim servicing api and document servicing api and get the 
        // remaining document count for the claim number
        /// </summary>
        public static int GetRemainingUploadDocumentCount(string personId, string claimNumber)
        {
            var claimServicing = new ClaimServicing(personId);
            var session = Task.Run(() => claimServicing.POST_CreateSession()).GetAwaiter().GetResult();
            var token = Task.Run(() => claimServicing.GET_ValidateUploadInvoice(claimNumber, session)).GetAwaiter().GetResult();

            var documentUpload = new DocumentUpload(personId);
            session = Task.Run(() => documentUpload.POST_CreateSession()).GetAwaiter().GetResult();
            return Task.Run(() => documentUpload.GET_UploadInvoiceConfig(token.Token, session)).GetAwaiter().GetResult();

        }

        /// <summary>
        /// Supports Motor Claim tests. Fetches all motor policy records 
        /// for the given Azure table.
        /// </summary>
        public static List<MotorPolicyEntity> AzureTableGetAllRecords(string tableName)
        {
            return AzureTableGetAllRecords<MotorPolicyEntity>(Config.Get().Azure.StorageMotorClaims, tableName);
        }

        /// <summary>
        /// Supports Member Refund tests. Fetches all test data from the Azure table.
        /// </summary>
        /// <returns></returns>
        public static List<MemberRefundEntity> AzureTableGetAllRefunds()
        {
            return AzureTableGetAllRecords<MemberRefundEntity>(Config.Get().Azure.StorageMemberRefund, "refunds");
        }

        private static List<T> AzureTableGetAllRecords<T>(AzureTable storageConfig, string tableName) where T : class, ITableEntity
        {
            var azureTable = new AzureTableOperation(storageConfig, tableName);
            return Task.Run(() => azureTable.GetAllEntries<T>()).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Add new policy records in the azure table
        /// </summary>
        /// <param name="storageConfig">Config values containing URI, and keys</param>
        /// <param name="tableName">the specific Azure table to be deleted and loaded with new values</param>
        /// <param name="newRowList">List of elements to load into the table</param>
        public static void AzureTableDeleteAndAddNewEntity<T>(AzureTable storageConfig, string tableName, List<T> newRowList) where T : class, ITableEntity
        {
            var azureTable = new AzureTableOperation(storageConfig, tableName);
            azureTable.DeleteAndAddNewEntityList(newRowList);
        }

        /// <summary>
        // Delete policy record from the azure table
        /// </summary>
        private static void AzureTableDeleteEntity(string pKey, string rKey, string tableName)
        {
            var azureTable = new AzureTableOperation(tableName);
            azureTable.DeleteTableRow(pKey, rKey);
        }

        public static void AzureTableDeleteEntityBasedOnPolicyNumber(string policy, string tableName)
        {
            var azureTable = new AzureTableOperation(tableName);

            azureTable.DeleteEntityBasedOnThePolicyNumber(policy);
        }
        #endregion

        #region Email Helper
        /// <summary>
        /// Method to find EFT links from claim update emails.
        /// </summary>
        /// <param name="claimNumber">Specific claim that the email should relate to.</param>
        /// <returns></returns>
        public static string GetClaimEFTLinkFromCashSettlementEmail(string claimNumber)
        {
            var emailHandler = new MailosaurEmailHandler();
            var email = Task.Run(() => emailHandler.FindEmailBySubject($"RAC Claim {claimNumber} Claim Update")).GetAwaiter().GetResult();

            return emailHandler.FindCSFSLink(email);
        }

        /// <summary>
        /// Method to find retrieve quote link from quote email.
        /// </summary>
        /// <param name="emailAddress">Policyholder email address where the quote email has sent</param>
        /// <returns></returns>
        public static string GetRetrieveQuoteLinkFromEmail( string emailAddress)
        {
            var emailHandler = new MailosaurEmailHandler();
            var email = Task.Run(() => emailHandler.FindEmailByRecipient(emailAddress)).GetAwaiter().GetResult();
            return emailHandler.FindRetrieveQuoteLink(email);
        }

        #endregion

        /// <summary>
        /// Adds a name:vaue pair to the given dictionary,
        /// if the value is not null or empty
        /// </summary>
        public static void AddParamIfNotEmpty(Dictionary<string, string> parameters, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
                parameters.Add(name, value);
        }

        /// <summary>
        /// This method evaluates if the given 'Postcode' is above the 26th parallel
        /// </summary>
        /// <param name="Postcode"></param>
        /// <returns>true/false</returns>
        public static bool IsPostcodeAbove26thParallel(string postCode) => Int32.Parse(postCode) > 6699;

        /// <summary>
        /// Helper that looks to determine if any of the contacts used as
        /// policyholders, are likely to have been member match contacts
        /// for non-disclosed memberships, and thus triggered a later applied
        /// discount on new business policy.
        /// 
        /// This method is intended to assist code in determining how strictly
        /// it should be asserting premium consistency through the new business
        /// flow, i.e. it is not a failure if premium changes at the end, if a
        /// member match is expected.
        /// </summary>
        /// <param name="policyholders"></param>
        /// <returns>true if a policyholder will trigger a member match and discount</returns>
        public static bool WouldAnyPolicyholdersTriggerRevisedPricing(List<Contact> policyholders)
        {
            var revisedPricingFlag = false;

            foreach (var policyholder in policyholders.Where(x => x.IsRACMember && x.SkipDeclaringMembership))
            {
                if (policyholder.MembershipTier == MembershipTier.Gold ||
                    policyholder.MembershipTier == MembershipTier.Silver ||
                    policyholder.MembershipTier == MembershipTier.Bronze)
                {
                    revisedPricingFlag = true;
                }
            }

            return revisedPricingFlag;
        }

        /// <summary>
        /// Gets the first dollar value amount from a string 
        /// </summary>
        public static string GetAnnualPremiumForMonthlyFrquency(string sentence)
        {
            // regular expression pattern to match currency values
            string pattern = @"\$\d+(\.\d{1,2})?";

            // Search for all matches in the sentence
            MatchCollection matches = Regex.Matches(sentence, pattern);
            return matches[0].Value.Replace("$","").Trim();          
        }

        /// <summary>
        /// Pulls random contact from associated contact list, where
        /// that contact is of desired RSA tier.
        /// That contact will be removed from the list so that other
        /// tests don't use it.
        /// </summary>
        /// <param name="desiredTier"></param>
        /// <returns>Randomly chosen contact of desired RSA tier</returns>
        /// <exception cref="Exception">If the list is empty, or does not find a contact with the desired tier</exception>
        public static Contact ConsumeContact(this List<Contact> contactCollection, params MembershipTier[] desiredTiers)
        {
            if (contactCollection.Count == 1)
            { Reporting.Error("No items remaining in list to consume"); }

            var contact = contactCollection.Where(x => desiredTiers.Contains(x.MembershipTier)).PickRandom();
            contactCollection.Remove(contact);

            return contact;
        }

        public static API_MemberCentralPersonV2Products FetchProductHoldingsFromMemberCentral(string personId)
        {
            return MemberCentral.GetInstance().GET_ProductHoldingsByPersonId(personId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// For the given Member contact, it will update the Member
        /// Central entry with the email provided.
        /// </summary>
        /// <param name="contactId">Shield Contact ID</param>
        /// <param name="email">email as a string</param>
        /// <returns>false if we failed to find the member in MC, otherwise true.</returns>
        public static bool OverrideMemberEmailInMemberCentralByContactID(string contactId, string email)
        {
            var getPersonResponse = Task.Run(() => MemberCentral.GetInstance().GET_PersonByShieldContactId(contactId)).GetAwaiter().GetResult();

            if (getPersonResponse == null)
            { return false; }
            
            Task.Run(() => MemberCentral.GetInstance().PUT_UpdateMemberEmailAddress(getPersonResponse, email)).GetAwaiter().GetResult();
            Reporting.Log($"Sync Shield contact with Member Central successfully for ContactId {contactId}. PersonId = {getPersonResponse.PersonId}");
            return true;
        }

        /// <summary>
        /// For the given Member contact, it will update the Member
        /// Central entry with the mobile phone number provided.
        /// </summary>
        /// <param name="contactId">Shield Contact ID</param>
        /// <param name="mobile">mobile phone number as a string</param>
        /// <returns>false if we failed to find the member in MC, otherwise true.</returns>
        public static bool OverrideMemberMobileInMemberCentralByContactID(string contactId, string mobile)
        {
            var getPersonResponse = Task.Run(() => MemberCentral.GetInstance().GET_PersonByShieldContactId(contactId)).GetAwaiter().GetResult();

            if (getPersonResponse == null)
            { return false; }

            Task.Run(() => MemberCentral.GetInstance().PUT_UpdateMemberMobile(getPersonResponse, mobile)).GetAwaiter().GetResult();
            Reporting.Log($"Sync Shield contact with Member Central successfully for ContactId {contactId}. PersonId = {getPersonResponse.PersonId}");
            return true;
        }

        /// <summary>
        /// Function to return the premium change in Whole amount
        /// if the premium happens to be a whole dollar amount, then Spark just omits the cents altogether.
        ///e.g.: a premium change of $20.00, is displayed just as $20.
        /// </summary>
        public static string GetCorrectedPremium(decimal amount)
        {
            return (amount.ToString().Replace(".00", "").Trim());
        }

        /// <summary>
        /// Allows retrieving a random item from a list. The
        /// returned element is removed from the list to prevent
        /// it from being returned in subsequent calls.
        /// </summary>
        /// <returns>Randomly chosen list item.</returns>
        /// <exception cref="Exception">If the list is empty.</exception>
        public static T ConsumeRandom<T>(this List<T> collection)
        {
            if (collection.Count == 1)
            { Reporting.Error("No items remaining in list to consume"); }

            var policy = collection.PickRandom();
            collection.Remove(policy);

            return policy;
        }

        #region SMS Helper 

        /// <summary>
        /// Method to find OTP in SMS from Mailosuar.
        /// </summary>
        /// </param>
        /// <returns>null if no OTP is found.</returns>
        public static string GetOTPFromSMS()
        {
            var emailHandler = new MailosaurEmailHandler();
            var email = Task.Run(() => emailHandler.FindSMSByBody("Your RAC - NPE verification code is:", retrySeconds: WaitTimes.T150SEC)).GetAwaiter().GetResult();
            return emailHandler.FindOTP(email);
        }

        /// <summary>
        /// Method to find OTP for registration of a new myRAC account from from Mailosuar email.
        /// </summary>
        /// </param>
        /// <returns>OTP code if found or NULL if no OTP is found.</returns>
        public static string GetMyRACRegistrationCodeFromEmail(string loginEmail)
        {
            var emailHandler = new MailosaurEmailHandler();
            var emailContent = Task.Run(() => emailHandler.FindEmailByRecipient(loginEmail, withinPastSeconds: WaitTimes.T30SEC, retrySeconds: WaitTimes.T90SEC)).GetAwaiter().GetResult();

            return emailHandler.FindOTP(emailContent);
        }
        #endregion
        #region Logging helpers for assertion messages
        /// <summary>
        /// Assertion message template for element is displayed
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsDisplayed(this string elementName) => $"{elementName} is displayed which is as expected";

        /// <summary>
        /// Assertion message template for element is not displayed
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsNotDisplayed(this string elementName) => $"{elementName} is not displayed which is as expected";

        /// <summary>
        /// Assertion message template for element is enabled
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsEnabled(this string elementName) => $"{elementName} is enabled which is as expected";

        /// <summary>
        /// Assertion message template for element is disabled
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsDisabled(this string elementName) => $"{elementName} is disabled which is as expected";

        /// <summary>
        /// Assertion message template for expected label text
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsExpectedLabelText(this string elementName) => $"{elementName} label text is as expected";

        /// <summary>
        /// Assertion message template for element is selected
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsSelected(this string elementName) => $"{elementName} is selected which is as expected";

        /// <summary>
        /// Assertion message template for element is not selected
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsNotSelected(this string elementName) => $"{elementName} is not selected which is as expected";

        /// <summary>
        /// Assertion message template for element/text was found
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsFound(this string elementName) => $"{elementName} was found which is as expected";

        /// <summary>
        /// Assertion message template for element/text was not found
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string IsNotFound(this string elementName) => $"{elementName} was not found which is as expected";
        #endregion

        #region Test Data Setup

        /// <summary>
        /// Read all the motor policy numbers from azure table storage and
        /// load all the policy details by calling GetPolicy API
        /// <returns></returns>
        public static List<MotorPolicy> GetAllMotorPolicyDetails(List<MotorPolicyEntity> motorPolicyEntities, string tableName)
        {
            var candidates = new List<MotorPolicy>();

            foreach (var policy in motorPolicyEntities)
            {
                var policyDetails = GetPolicyDetails(policy.PolicyNumber);
                var policyHolders = FetchPolicyContacts(policyDetails);

                // Weed out unusable records.
                if (policyHolders == null ||
                    string.Equals(policyDetails.Status, "CancelledPolicy") ||
                    policyDetails.Covers == null ||
                    policyDetails.Covers.Count == 0)
                {
                    DataHelper.AzureTableDeleteEntity(policy.PartitionKey, policy.RowKey, tableName);
                    continue;
                }

                candidates.Add(new MotorPolicy
                {
                    PolicyNumber = policyDetails.PolicyNumber,
                    PolicyHolders = policyHolders,
                    CoverType = policy.CoverType,
                    LastEndorsementDate = policyDetails.EndorsementStartDate,
                    Excess = (int)policyDetails.Covers.FirstOrDefault().StandardExcess,
                    SumInsured = (int)policyDetails.MotorAsset.SumInsured,
                    PolicyStartDate = policyDetails.PolicyStartDate,
                    RiskAddress = policyDetails.MotorAsset.Address,
                    HasHireCarCover = policyDetails.HasHireCarCover,
                    Vehicle = MapCarWithGetVehicleAPI(policyDetails.GetVehicleId(), policyDetails.MotorAsset.RegistrationNumber, policy.IsEV)
                });
            }

            return candidates;
        }

        public static MotorPolicy GetMotorPolicyDetailsFromEntity(MotorPolicyEntity policy, string tableName)
        {
            MotorPolicy candidate = new MotorPolicy();
            try
            {
                var policyDetails = GetPolicyDetails(policy.PolicyNumber);
                var policyHolders = FetchPolicyContacts(policyDetails);

                // Weed out unusable records.
                if (policyHolders.Count        == 0 ||
                    string.Equals(policyDetails.Status, "CancelledPolicy") ||
                    policyDetails.Covers       == null ||
                    policyDetails.Covers.Count == 0)
                {
                    Reporting.Log($"{policy.PolicyNumber} was either cancelled/no covers/no policyholders, we'll remove it from Azure table.");
                    AzureTableDeleteEntity(policy.PartitionKey, policy.RowKey, tableName);
                    return null;
                }

                candidate =new MotorPolicy
                {
                    PolicyNumber    = policyDetails.PolicyNumber,
                    PolicyHolders   = policyHolders,
                    CoverType       = policy.CoverType,
                    LastEndorsementDate = policyDetails.EndorsementStartDate,
                    Excess          = (int)policyDetails.Covers.FirstOrDefault().StandardExcess,
                    SumInsured      = (int)policyDetails.MotorAsset.SumInsured,
                    PolicyStartDate = policyDetails.PolicyStartDate,
                    RiskAddress     = policyDetails.MotorAsset.Address,
                    HasHireCarCover = policyDetails.HasHireCarCover,
                    Vehicle         = MapCarWithGetVehicleAPI(policyDetails.GetVehicleId(), policyDetails.MotorAsset.RegistrationNumber, policy.IsEV)
                };
            }
            catch
            {
                Reporting.Log($"We had a error occur while getting details for {policy.PolicyNumber}, we'll remove it from Azure table.");
                AzureTableDeleteEntity(policy.PartitionKey, policy.RowKey, tableName);
                return null;
            }

            return candidate;
        }

        /// <summary>
        /// Notice: Returns only one policy details
        /// <returns></returns>
        public static MotorPolicy GetSingleMotorPolicyDetail(MotorPolicyEntity motorPolicy, string tableName)
        {
            var candidate = new MotorPolicy();

            var policyDetails = GetPolicyDetails(motorPolicy.PolicyNumber);
            var policyHolders = FetchPolicyContacts(policyDetails);

            if (policyHolders != null)
            {
                candidate.PolicyNumber = policyDetails.PolicyNumber;
                candidate.PolicyHolders = policyHolders;
                candidate.CoverType = motorPolicy.CoverType;
                candidate.LastEndorsementDate = policyDetails.EndorsementStartDate;
                candidate.Excess = (int)policyDetails.Covers.FirstOrDefault().StandardExcess;
                candidate.SumInsured = (int)policyDetails.MotorAsset.SumInsured;
                candidate.PolicyStartDate = policyDetails.PolicyStartDate;
                candidate.RiskAddress = policyDetails.MotorAsset.Address;
                candidate.HasHireCarCover = policyDetails.HasHireCarCover;
                candidate.Vehicle = MapCarWithGetVehicleAPI(policyDetails.GetVehicleId(), policyDetails.MotorAsset.RegistrationNumber, motorPolicy.IsEV);
            }
            else
            {
                DataHelper.AzureTableDeleteEntity(motorPolicy.PartitionKey, motorPolicy.RowKey, tableName);
            }

            return candidate;
        }        

        /// <summary>
        /// Returns the list of policy holders and co-policy holders on a policy
        /// </summary>
        /// <param name="policyDetails"> Response from GetPolicy API call</param>
        /// <returns>List of PolicyContactDB, if there are no policyholders, an empty list is returned.</returns>
        public static List<PolicyContactDB> FetchPolicyContacts(GetQuotePolicy_Response policyDetails)
        {
            var policyContacts = new List<PolicyContactDB>();

            var policyHolder = MapPolicyContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber, ContactRole.PolicyHolder);            
            
            if (policyHolder != null)
            {
                policyContacts.Add(policyHolder);
            };            

            if (!policyDetails.PolicyCoOwners.IsNullOrEmpty())
            {
                foreach (var coPolicyOwner in policyDetails.PolicyCoOwners)
                {
                    policyContacts.Add(MapPolicyContactWithPersonAPI(coPolicyOwner.Id.ToString(), coPolicyOwner.ContactExternalNumber, ContactRole.CoPolicyHolder));
                }
            }

            // If policy have additional driver who is not a policy holder or co-owner
            // then sync all additional drivers with member central
            if (policyContacts.Count() > 0 && policyContacts.Any())
            {
                foreach (var driver in policyDetails.MotorAsset.Drivers)
                {
                    if (policyContacts.Any(p => p.ExternalContactNumber == driver.ContactExternalNumber))
                    { continue; }  // We have already synced that person

                    var contact = GetContactDetailsViaExternalContactNumber(driver.ContactExternalNumber);
                    MemberCentral.SyncMemberCentralWithShield(contact.Id);
                }
            }

            return policyContacts;
        }

        /// <summary>
        /// Map the PolicyContactDB object with the Member Central GetPerson API call
        /// If the sync fail with MC because of multi match or sync error then it will 
        /// return a null Contact
        /// </summary>
        /// <param name="contactId"> Shield contact id</param>
        /// <param name="contactExternalNumber"> Shield external contact number, we only need it for mapping purpose
        /// as member central has no information of Shield contact external numbers</param>
        /// <param name="contactRole"> Contact role for the policy holders</param>
        /// <returns>PolicyContactDB object</returns>
        public static PolicyContactDB MapPolicyContactWithPersonAPI(string contactId, string contactExternalNumber, ContactRole contactRole)
        {
            PolicyContactDB policyContact = null;
            if (MemberCentral.SyncMemberCentralWithShield(contactId))
            {
                policyContact = PolicyContactDB.Init(Contact.InitFromMCByShieldId(contactId, contactExternalNumber));
                if (policyContact != null)
                {
                    policyContact.ContactRoles = new List<ContactRole> { contactRole };
                }                             
            }
            return policyContact;
        }

        /// <summary>
        /// Map the Contact object with the Member Central GetPerson API call
        /// If the sync fail with MC because of multi match or sync error then it will 
        /// return a null Contact
        /// </summary>
        /// <param name="contactId"> Shield contact id</param>
        /// <param name="contactExternalNumber"> Shield external contact number, we only need it for mapping purpose
        /// as member central has no information of Shield contact external numbers</param>
        /// <returns>Contact object</returns>
        public static Contact MapContactWithPersonAPI(string contactId, string contactExternalNumber = null)
        {
            Contact policyContact = null;
            if (MemberCentral.SyncMemberCentralWithShield(contactId))
            {
                policyContact = Contact.InitFromMCByShieldId(contactId, contactExternalNumber);               
            }
            return policyContact;
        }

        /// <summary>
        /// Returns the vehicle details from a given motor policy number
        /// </summary>
        /// <param name="vehicleId"> Vecicle id from the Get Policy response</param>
        /// <param name="rego"> Registration number from the Get Policy response</param>
        /// <param name="isEV"> isEV flag from Azure table storage</param>
        /// <returns>Car</returns>
        public static Car MapCarWithGetVehicleAPI(string vehicleId, string rego, bool isEV)
        {
            var vehicleDetails = GetVehicleDetails(vehicleId).Vehicles.FirstOrDefault();
            return new Car()
            {
                Year = vehicleDetails.ModelYear,
                Make = vehicleDetails.MakeDescription,
                Model = vehicleDetails.ModelDescription,
                Body = vehicleDetails.VehicleSubTypeDescription,
                Transmission = vehicleDetails.TransmissionDescription,
                Registration = rego,                
                IsElectricVehicle = isEV,
            };
        }

        #endregion
    }
}
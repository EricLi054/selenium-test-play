using System;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyPet;

namespace Rac.TestAutomation.Common.TestData.Quote
{
    public class PetBuilder : EntityBuilder<QuotePet, PetBuilder>
    {
        // Min pet age is 8 weeks (day over 8weeks* 7days), hence 57
        private static readonly int MINIMUM_PET_AGE_DAYS = 57;

        public PetBuilder() { }

        public PetBuilder WithResidingSuburb(string suburb, string postcode)
        {
            var address = new Address()
            {
                Suburb = suburb,
                PostCode = postcode
            };
            return WithResidingSuburbFromAddress(address);
        }

        public PetBuilder WithResidingSuburbFromAddress(Address address)
        {
            Set(x => x.ResidingAddress, address);
            return this;
        }

        public PetBuilder WithInsurer(string insurer)
        {
            Set(x => x.CurrentInsurer, insurer);
            return this;
        }

        public PetBuilder WithoutInsurer()
        {
            Set(x => x.CurrentInsurer, null);
            return this;
        }

        public PetBuilder WithRandomInsurer()
        {
            return WithInsurer(GetRandomInsurer());
        }

        public PetBuilder WithTypeAndBreed(PetType type, string breed)
        {
            Set(x => x.Type,  type);
            Set(x => x.Breed, breed);
            return this;
        }

        public PetBuilder WithTypeAndRandomBreed(PetType type)
        {
            return WithTypeAndBreed(type, GetARandomBreedBasedOnPetType(type));
        }

        public PetBuilder WithRandomTypeAndBreed()
        {
            var randType = Randomiser.Get.Next(2) == 0 ? PetType.Dog : PetType.Cat;

            return WithTypeAndRandomBreed(randType);
        }

        public PetBuilder WithRandomPetName()
        {
            return WithPetName(DataHelper.RandomLetters(3, 20));
        }

        public PetBuilder WithPetName(string name)
        {
            Set(x => x.Name, name);
            return this;
        }

        public PetBuilder WithPetBirthdate(DateTime petDoB)
        {
            Set(x => x.DateOfBirth, petDoB);
            return this;
        }

        /// <summary>
        /// Generate a random date of birth between 8 weeks and under 9 years.
        /// </summary>
        /// <returns></returns>
        public PetBuilder WithRandomPetBirthdate()
        {
            var birthdate = DataHelper.RandomDoB(0, 8);
            // Minimum allowable pet age is measured in weeks, so check to make sure we're not under that.
            if (DateTime.Now.Subtract(birthdate).TotalDays < MINIMUM_PET_AGE_DAYS)
                birthdate = DateTime.Now.Date.AddDays(-MINIMUM_PET_AGE_DAYS);

            return WithPetBirthdate(birthdate);
        }

        public PetBuilder WithRandomPetGender()
        {
            var randGender = Randomiser.Get.Next(2) == 0 ? Gender.Male : Gender.Female;

            return WithPetGender(randGender);
        }

        public PetBuilder WithPetGender(Gender gender)
        {
            Set(x => x.Gender, gender);
            return this;
        }

        public PetBuilder WithRandomPetSterilisationStatus()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithPetSterilisationStatus(randValue);
        }

        public PetBuilder WithPetSterilisationStatus(bool isSterilised)
        {
            Set(x => x.IsSterilised, isSterilised);
            return this;
        }

        public PetBuilder WithRandomPetPreexistingConditions()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithPetPreexistingConditions(randValue);
        }

        public PetBuilder WithPetPreexistingConditions(bool hasPrexistingCondition)
        {
            Set(x => x.HasPrexistingCondition, hasPrexistingCondition);
            return this;
        }

        public PetBuilder WithRandomIsWorkingAnimalStatus()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithIsWorkingAnimalStatus(randValue);
        }

        public PetBuilder WithIsWorkingAnimalStatus(bool isWorkingAnimal)
        {
            Set(x => x.IsWorkingAnimal, isWorkingAnimal);
            return this;
        }

        public PetBuilder WithRandomOwnedByBusinessStatus()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithOwnedByBusinessStatus(randValue);
        }

        public PetBuilder WithOwnedByBusinessStatus(bool isOwnedByBusiness)
        {
            Set(x => x.IsBusinessOwned, isOwnedByBusiness);
            return this;
        }

        public PetBuilder WithRegularVet(string vetName, string vetAddress)
        {
            Set(x => x.VetName, vetName);
            Set(x => x.VetAddress, vetAddress);
            return this;
        }

        public PetBuilder WithoutRegularVet()
        {
            Set(x => x.VetName, null);
            Set(x => x.VetAddress, null);
            return this;
        }

        public PetBuilder WithPolicyHolder(Contact contact)
        {
            Set(x => x.PolicyHolder, contact);
            return this;
        }

        public PetBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        /// <summary>
        /// Sets the provided contact as payer, and randomizes the
        /// payment terms. Will override any previously set payment
        /// details.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public PetBuilder WithPayer(Contact contact)
        {
            Set(x => x.PayMethod, new Payment(contact));
            return this;
        }

        public PetBuilder WithRandomPaymentMethod()
        {
            Set(x => x.PayMethod, new Payment(Get(x => x.PolicyHolder)));
            return this;
        }

        public PetBuilder WithPaymentMethod(bool useDirectDebit)
        {
            var currentPaymentMethod = Get(x => x.PayMethod);
            if (currentPaymentMethod == null)
            {
                currentPaymentMethod = new Payment(Get(x => x.PolicyHolder));
            }

            currentPaymentMethod.IsPaymentByBankAccount = useDirectDebit;

            Set(x => x.PayMethod, currentPaymentMethod);
            return this;
        }

        public PetBuilder WithPolicyStartDate(DateTime date)
        {
            Set(x => x.StartDate, date);
            return this;
        }

        public PetBuilder WithTLCCover(bool addTlc)
        {
            Set(x => x.AddTlc, addTlc);
            return this;
        }

        /// <summary>
        /// This sets the excess to "null" which tells the test to accept
        /// whatever Shield defaults the quote to based on rating.
        /// </summary>
        /// <returns></returns>
        public PetBuilder WithDefaultExcess()
        {
            Set(x => x.Excess, null);
            return this;
        }

        public PetBuilder WithExcess(string excess)
        {
            Set(x => x.Excess, excess);
            return this;
        }

        public PetBuilder InitialisePetWithRandomData(Contact policyholder)
        {
            var quotePetBuilder = WithRandomTypeAndBreed()
                .WithRandomInsurer()
                .WithRandomPetName()
                .WithRandomPetBirthdate()
                .WithPolicyHolder(policyholder)
                .WithResidingSuburbFromAddress(policyholder.MailingAddress)
                .WithPolicyStartDate(DateTime.Now)
                .WithTLCCover(false)
                .WithDefaultExcess()
                .WithRandomPetGender()
                .WithRandomPetSterilisationStatus()
                .WithPetPreexistingConditions(false)
                .WithIsWorkingAnimalStatus(false)
                .WithOwnedByBusinessStatus(false)
                .WithoutRegularVet()
                .WithRandomInsurer()
                .WithRandomPaymentMethod();

            return quotePetBuilder;
        }

        protected override QuotePet BuildEntity()
        {
            return new QuotePet
            {
                Type  = GetOrDefault(x => x.Type),
                Breed = GetOrDefault(x => x.Breed),
                DateOfBirth = GetOrDefault(x => x.DateOfBirth),
                Name = GetOrDefault(x => x.Name),
                ResidingAddress = GetOrDefault(x => x.ResidingAddress),
                CurrentInsurer = GetOrDefault(x => x.CurrentInsurer),
                PolicyHolder = GetOrDefault(x => x.PolicyHolder),
                Gender = GetOrDefault(x => x.Gender),
                HasPrexistingCondition = GetOrDefault(x => x.HasPrexistingCondition),
                IsSterilised = GetOrDefault(x => x.IsSterilised),
                IsWorkingAnimal = GetOrDefault(x => x.IsWorkingAnimal),
                IsBusinessOwned = GetOrDefault(x => x.IsBusinessOwned),
                VetName = GetOrDefault(x => x.VetName),
                VetAddress = GetOrDefault(x => x.VetAddress),
                StartDate = GetOrDefault(x => x.StartDate),
                AddTlc = GetOrDefault(x => x.AddTlc),
                Excess = GetOrDefault(x => x.Excess),
                PayMethod = GetOrDefault(x => x.PayMethod)
            };
        }

        private string GetRandomInsurer()
        {
            var insurerOptions = new[]
            {
                "AUSTRALIA POST",
                "AUSTRALIAN SENIORS PET INSURANCE",
                "BOW WOW MEOW",
                "BUPA",
                "GREENCROSS PET INSURANCE",
                "GUIDE DOGS PET INSURANCE",
                "HCF",
                "MEDIBANK",
                "NOT PREVIOUSLY INSURED",
                "PET INSURANCE AUSTRALIA",
                "PETBARN PET INSURANCE",
                "PETCOVER PET INSURANCE",
                "PETMED PET INSURANCE",
                "PETPLAN PET INSURANCE",
                "PETSECURE PET INSURANCE",
                "PRIME PET INSURANCE",
                "PROSURE PET INSURANCE",
                "RSPCA",
                "OTHER PET INSURER"
            };

            return insurerOptions.OrderBy(t => Guid.NewGuid()).First();
        }

        private string GetARandomBreedBasedOnPetType(PetType type)
        {
            return type == PetType.Dog ? GetRandomDogBreed() : GetRandomCatBreed();
        }

        private string GetRandomCatBreed()
        {
            var catOptions = new[]
            {
                "ABYSSINIAN",
                "ABYSSINIAN CROSS",
                "AMERICAN (S/W HAIRED)",
                "ANGORA",
                "ASIAN SHORTHAIR",
                "AUSTRALIAN MIST",
                "AUSTRALIAN MIST CROSS",
                "BALINESE",
                "BALINESE CROSS",
                "BENGAL",
                "BENGAL CROSS",
                "BIRMAN",
                "BIRMAN CROSS",
                "BLUE BURMESE",
                "BLUE-POINTED SIAMESE",
                "BOMBAY",
                "BRAZILIAN SHORTHAIR",
                "BRITISH BLUE",
                "BRITISH BLUE CROSS",
                "BRITISH CREAM",
                "BRITISH LONGHAIR",
                "BRITISH LONGHAIR CROSS",
                "BRITISH SHORTHAIR",
                "BRITISH SHORTHAIR CROSS",
                "BRITISH TIP",
                "BROWN BURMESE",
                "BROWN TABBY",
                "BURMESE",
                "BURMESE CROSS",
                "BURMILLA",
                "BURMILLA CROSS",
                "CHINCHILLA",
                "CHINCHILLA CROSS",
                "CHINCHILLA PERSIAN",
                "CHOCOLATE BURMESE",
                "CHOCOLATE-POINTED SIAMESE",
                "CORNISH REX",
                "CROSSBREED",
                "DEVON REX",
                "DEVON REX CROSS",
                "DOMESTIC",
                "DOMESTIC LONG-HAIRED",
                "DOMESTIC MEDIUM HAIRED",
                "DOMESTIC SHORTHAIRED",
                "EUROPEAN SHORTHAIR",
                "EXOTIC",
                "GINGER",
                "GOLDEN TABBY",
                "HAVANA",
                "HIMALAYAN",
                "HIMALAYAN CROSS",
                "HIMALAYAN PERSIAN",
                "KORAT",
                "LA PERM",
                "LAYANESE",
                "LILAC BURMESE",
                "LILAC-POINTED SIAMESE",
                "MAINE COON",
                "MANX",
                "MANX CROSS",
                "MOGGY",
                "MUNCHKIN",
                "NORWEGIAN FOREST CAT",
                "OCICAT",
                "ORIENTAL",
                "ORIENTAL CROSS",
                "ORIENTAL LONGHAIR",
                "ORIENTAL SHORTHAIR",
                "PERSIAN",
                "PERSIAN CROSS",
                "RAGDOLL",
                "RAGDOLL CROSS",
                "RED-POINTED SIAMESE",
                "RUSSIAN",
                "RUSSIAN BLACK",
                "RUSSIAN BLUE",
                "RUSSIAN BLUE CROSS",
                "SCOTTISH FOLD",
                "SCOTTISH FOLD (S/L HAIRED)",
                "SCOTTISH FOLD (SH, LH)",
                "SCOTTISH FOLD CROSS",
                "SCOTTISH SHORTHAIR",
                "SEAL-POINTED SIAMESE",
                "SELKIRK",
                "SELKIRK REX",
                "SIAMESE",
                "SIAMESE CROSS",
                "SIBERIAN",
                "SILVER TABBY",
                "SINGAPURA",
                "SNOWSHOE",
                "SOMALI",
                "SPHYNX",
                "TABBY",
                "TABBY-POINTED SIAMESE",
                "TIFFANIE",
                "TIFFANIE CROSS",
                "TONKINESE",
                "TORTIE-POINTED SIAMESE",
                "TORTOISESHELL",
                "TORTOISESHELL AND WHITE",
                "TOYGER",
                "TURKISH ANGORA",
                "TURKISH VAN"
            };

            return catOptions.OrderBy(t => Guid.NewGuid()).First();
        }

        private string GetRandomDogBreed()
        {
            var dogOptions = new[]
            {
                "ABERDEEN TERRIER",
                "ABERDEEN TERRIER CROSS",
                "AFFENPINSCHER",
                "AFFENPINSCHER CROSS",
                "AFGHAN HOUND",
                "AFGHAN HOUND CROSS",
                "AIREDALE TERRIER",
                "AIREDALE TERRIER CROSS",
                "AKITA INU",
                "AKITA INU CROSS",
                "ALASKAN HUSKY",
                "ALASKAN HUSKY CROSS",
                "ALASKAN MALAMUTE",
                "ALASKAN MALAMUTE CROSS",
                "ALSATIAN",
                "ALSATIAN CROSS",
                "AMERICAN AKITA",
                "AMERICAN AKITA CROSS",
                "AMERICAN BULLDOG",
                "AMERICAN BULLDOG CROSS",
                "AMERICAN COCKER SPANIEL",
                "AMERICAN COCKER SPANIEL CROSS",
                "AMERICAN ESKIMO",
                "AMERICAN ESKIMO CROSS",
                "AMERICAN FOXHOUND",
                "AMERICAN FOXHOUND CROSS",
                "AMERICAN STAFFORDSHIRE TERRIER",
                "AMERICAN STAFFORDSHIRE TERRIER CROSS",
                "AMERICAN WATER SPANIEL",
                "AMERICAN WATER SPANIEL CROSS",
                "ANATOLIAN KARABASH",
                "ANATOLIAN KARABOSH CROSS",
                "ANATOLIAN SHEPHERD DOG",
                "ANATOLIAN SHEPHERD DOG CROSS",
                "ANTARCTIC HUSKY",
                "ANTARCTIC HUSKY CROSS",
                "ANTARTIC HUSKY CROSS",
                "AUSTRALIAN BULLDOG",
                "AUSTRALIAN BULLDOG CROSS",
                "AUSTRALIAN BULLDOG MINIATURE",
                "AUSTRALIAN BULLDOG MINIATURE CROSS",
                "AUSTRALIAN CATTLE DOG",
                "AUSTRALIAN CATTLE DOG CROSS",
                "AUSTRALIAN HEELER",
                "AUSTRALIAN HEELER CROSS",
                "AUSTRALIAN KELPIE SHEEPDOG",
                "AUSTRALIAN KELPIE SHEEPDOG CROSS",
                "AUSTRALIAN KOOLIE",
                "AUSTRALIAN KOOLIE CROSS",
                "AUSTRALIAN SHEPHERD",
                "AUSTRALIAN SHEPHERD CROSS",
                "AUSTRALIAN SILKY TERRIER",
                "AUSTRALIAN SILKY TERRIER CROSS",
                "AUSTRALIAN TERRIER",
                "AUSTRALIAN TERRIER CROSS",
                "BASENJI",
                "BASENJI CROSS",
                "BASSET FAUVE DE BRETAGNE",
                "BASSET FAUVE DE BRETAGNE CROSS",
                "BASSET HOUND",
                "BASSET HOUND CROSS",
                "BEAGLE",
                "BEAGLE CROSS",
                "BEARDED COLLIE",
                "BEARDED COLLIE CROSS",
                "BEDLINGTON TERRIER",
                "BEDLINGTON TERRIER CROSS",
                "BELGIAN SHEPHERD",
                "BELGIAN SHEPHERD - LAEKENOIS/GROENENDAEL",
                "BELGIAN SHEPHERD - LAEKENOIS/GROENENDAEL CROSS",
                "BELGIAN SHEPHERD - MALINOIS",
                "BELGIAN SHEPHERD - MALINOIS CROSS",
                "BELGIAN SHEPHERD - TERVUEREN",
                "BELGIAN SHEPHERD - TERVUEREN CROSS",
                "BELGIAN SHEPHERD CROSS",
                "BERNESE MOUNTAIN DOG",
                "BERNESE MOUNTAIN DOG CROSS",
                "BICHON FRISE",
                "BICHON FRISE CROSS",
                "BLACK AND TAN COONHOUND",
                "BLACK AND TAN COONHOUND CROSS",
                "BLACK RUSSIAN TERRIER",
                "BLACK RUSSIAN TERRIER CROSS",
                "BLOODHOUND",
                "BLOODHOUND CROSS",
                "BLUE TICK COONHOUND",
                "BLUE TICK COONHOUND CROSS",
                "BOERBOEL",
                "BOERBOEL CROSS",
                "BORDEAUX MASTIFF",
                "BORDEAUX MASTIFF CROSS",
                "BORDER COLLIE",
                "BORDER COLLIE CROSS",
                "BORDER TERRIER",
                "BORDER TERRIER CROSS",
                "BORZOI",
                "BORZOI CROSS",
                "BOSTON TERRIER",
                "BOSTON TERRIER CROSS",
                "BOUVIER DES FLANDRES",
                "BOUVIER DES FLANDRES CROSS",
                "BOXER",
                "BOXER CROSS",
                "BRIARD",
                "BRIARD CROSS",
                "BRITISH BULLDOG",
                "BRITISH BULLDOG CROSS",
                "BRITTANY SPANIEL",
                "BRITTANY SPANIEL CROSS",
                "BULL ARAB",
                "BULL ARAB CROSS",
                "BULL TERRIER",
                "BULL TERRIER CROSS",
                "BULLMASTIFF",
                "BULLMASTIFF CROSS",
                "CAIRN TERRIER",
                "CAIRN TERRIER CROSS",
                "CANAAN",
                "CANAAN CROSS",
                "CANE CORSO",
                "CAO DE SERRA DE AIRES",
                "CAO DE SERRA DE AIRES CROSS",
                "CARDIGAN CORGI",
                "CARDIGAN CORGI CROSS",
                "CAVALIER KING CHARLES SPANIEL",
                "CAVALIER KING CHARLES SPANIEL CROSS",
                "CAVOODLE",
                "CENTRAL ASIAN OVTCHARKA",
                "CENTRAL ASIAN OVTCHARKA CROSS",
                "CENTRAL ASIAN SHEPHERD",
                "CENTRAL ASIAN SHEPHERD CROSS",
                "CESKY TERRIER",
                "CESKY TERRIER CROSS",
                "CHEASAPEAKE BAY RETRIEVER",
                "CHEASAPEAKE BAY RETRIEVER CROSS",
                "CHIHUAHUA",
                "CHIHUAHUA CROSS",
                "CHINESE CRESTED DOG",
                "CHINESE CRESTED DOG CROSS",
                "CHOW CHOW",
                "CHOW CHOW CROSS",
                "CLUMBER SPANIEL",
                "CLUMBER SPANIEL CROSS",
                "COCKER SPANIEL",
                "COCKER SPANIEL CROSS",
                "COLLIE - ROUGH",
                "COLLIE - ROUGH CROSS",
                "COLLIE - SMOOTH",
                "COLLIE - SMOOTH CROSS",
                "COLLIE (ROUGH/SMOOTH) CROSS",
                "CORGI",
                "CORGI CROSS",
                "COTON DE TULEAR",
                "COTON DE TULEAR CROSS",
                "CURLY COATED RETRIEVER",
                "CURLY COATED RETRIEVER CROSS",
                "DACHSHUND",
                "DACHSHUND - LONG HAIRED",
                "DACHSHUND - LONG HAIRED CROSS",
                "DACHSHUND - SMOOTH HAIRED",
                "DACHSHUND - SMOOTH HAIRED CROSS",
                "DACHSHUND CROSS",
                "DACHSHUND WIRE HAIRED",
                "DACHSHUND WIRE HAIRED CROSS",
                "DALMATIAN",
                "DALMATIAN CROSS",
                "DANDIE DINMONT TERRIER",
                "DANDIE DINMONT TERRIER CROSS",
                "DANISH-SWEDISH FARM DOG",
                "DANISH-SWEDISH FARM DOG CROSS",
                "DEERHOUND",
                "DEERHOUND CROSS",
                "DOBERMAN PINSCHER",
                "DOBERMAN PINSCHER CROSS",
                "DOBERMANN",
                "DOBERMANN CROSS",
                "DOGUE DE BORDEAUX",
                "DOGUE DE BORDEAUX CROSS",
                "DREVER",
                "DREVER CROSS",
                "DUTCH SHEPHERD",
                "DUTCH SHEPHERD CROSS",
                "EGYPTIAN PHAROAH HOUND",
                "EGYPTIAN PHAROAH HOUND CROSS",
                "ELKHOUND",
                "ELKHOUND CROSS",
                "ENGLISH MASTIFF",
                "ENGLISH MASTIFF CROSS",
                "ENGLISH POINTER",
                "ENGLISH POINTER CROSS",
                "ENGLISH SETTER",
                "ENGLISH SETTER CROSS",
                "ENGLISH SPRINGER SPANIEL",
                "ENGLISH SPRINGER SPANIEL CROSS",
                "ENGLISH TOY TERRIER",
                "ENGLISH TOY TERRIER CROSS",
                "ESTRELA",
                "ESTRELA CROSS",
                "FIELD SPANIEL",
                "FIELD SPANIEL CROSS",
                "FINNISH LAPPHUND",
                "FINNISH LAPPHUND CROSS",
                "FINNISH SPITZ",
                "FINNISH SPITZ CROSS",
                "FLAT COATED RETRIEVER",
                "FLAT COATED RETRIEVER CROSS",
                "FOX HOUND",
                "FOX HOUND CROSS",
                "FOX TERRIER SMOOTH",
                "FOX TERRIER SMOOTH CROSS",
                "FOX TERRIER WIRE",
                "FOX TERRIER WIRE CROSS",
                "FRENCH BULLDOG",
                "FRENCH BULLDOG CROSS",
                "FRENCH POODLE",
                "FRENCH POODLE CROSS",
                "GAZELLE HOUND",
                "GAZELLE HOUND CROSS",
                "GERMAN HUNTING TERRIER",
                "GERMAN HUNTING TERRIER CROSS",
                "GERMAN KOOLIE",
                "GERMAN KOOLIE CROSS",
                "GERMAN PINSCHER",
                "GERMAN PINSCHER CROSS",
                "GERMAN SCHNAUZER",
                "GERMAN SCHNAUZER CROSS",
                "GERMAN SHEPHERD",
                "GERMAN SHEPHERD CROSS",
                "GERMAN SHORT HAIRED POINTER",
                "GERMAN SHORT HAIRED POINTER CROSS",
                "GERMAN SPITZ",
                "GERMAN SPITZ CROSS",
                "GERMAN WIRE HAIRED POINTER",
                "GERMAN WIRE HAIRED POINTER CROSS",
                "GOLDEN DOODLE (GROODLE)",
                "GOLDEN RETRIEVER",
                "GOLDEN RETRIEVER CROSS",
                "GORDON SETTER",
                "GORDON SETTER CROSS",
                "GRAND BASSET GRIFFON VENDEEN",
                "GRAND BASSET GRIFFON VENDEEN CROSS",
                "GREAT DANE",
                "GREAT DANE CROSS",
                "GREAT PYRENEAN",
                "GREAT PYRENEAN CROSS",
                "GREEK HAREHOUND",
                "GREEK HAREHOUND CROSS",
                "GREYHOUND",
                "GREYHOUND CROSS",
                "GRIFFON",
                "GRIFFON BRABANCON",
                "GRIFFON BRABANCON CROSS",
                "GRIFFON BRUXELLOIS",
                "GRIFFON BRUXELLOIS CROSS",
                "GRIFFON CROSS",
                "HAMILTONSTOVARE",
                "HAMILTONSTOVARE CROSS",
                "HARRIER",
                "HARRIER CROSS",
                "HAVANESE",
                "HAVANESE CROSS",
                "HOVAWART",
                "HOVAWART CROSS",
                "HUNGARIAN KUVASZ",
                "HUNGARIAN KUVASZ CROSS",
                "HUNGARIAN PULI",
                "HUNGARIAN PULI CROSS",
                "HUNGARIAN VIZSLA",
                "HUNGARIAN VIZSLA CROSS",
                "HUNTAWAY",
                "HUNTAWAY CROSS",
                "HYGEN HOUND",
                "HYGEN HOUND CROSS",
                "IBIZAN HOUND",
                "IBIZAN HOUND CROSS",
                "IRISH RED AND WHITE SETTER",
                "IRISH RED AND WHITE SETTER CROSS",
                "IRISH SETTER",
                "IRISH SETTER CROSS",
                "IRISH TERRIER",
                "IRISH TERRIER CROSS",
                "IRISH WATER SPANIEL",
                "IRISH WATER SPANIEL CROSS",
                "IRISH WOLFHOUND",
                "IRISH WOLFHOUND CROSS",
                "ITALIAN CANE CORSO",
                "ITALIAN CANE CORSO CROSS",
                "ITALIAN GREYHOUND",
                "ITALIAN GREYHOUND CROSS",
                "ITALIAN LAGOTTO RAMGNOLO",
                "ITALIAN LAGOTTO RAMGNOLO CROSS",
                "ITALIAN POINTER",
                "ITALIAN POINTER CROSS",
                "ITALIAN SHEEPDOG",
                "ITALIAN SHEEPDOG CROSS",
                "ITALIAN SPINONE",
                "ITALIAN SPINONE CROSS",
                "JACK RUSSELL TERRIER",
                "JACK RUSSELL TERRIER CROSS",
                "JAPANESE AKITA",
                "JAPANESE AKITA CROSS",
                "JAPANESE CHIN",
                "JAPANESE CHIN CROSS",
                "JAPANESE SPITZ",
                "JAPANESE SPITZ CROSS",
                "KANGAL",
                "KANGAL CROSS",
                "KEESHOND",
                "KEESHOND CROSS",
                "KELPIE",
                "KELPIE CROSS",
                "KERRY BLUE",
                "KERRY BLUE CROSS",
                "KING CHARLES SPANIEL",
                "KING CHARLES SPANIEL CROSS",
                "KOMONDOR",
                "KOMONDOR CROSS",
                "KUVASZ",
                "KUVASZ CROSS",
                "LABRADOODLE",
                "LABRADOODLE CROSS",
                "LABRADOR",
                "LABRADOR CROSS",
                "LABRADOR RETRIEVER",
                "LABRADOR RETRIEVER CROSS",
                "LAGOTTO ROM",
                "LAGOTTO ROM CROSS",
                "LAKELAND TERRIER",
                "LAKELAND TERRIER CROSS",
                "LEONBERGER",
                "LEONBERGER CROSS",
                "LHASA APSO",
                "LHASA APSO CROSS",
                "LOUISIANA CATAHOULA LEOPARD DOG",
                "LOUISIANA CATAHOULA LEOPARD DOG CROSS",
                "LOWCHEN",
                "LOWCHEN CROSS",
                "LURCHER",
                "LURCHER CROSS",
                "MALINOIS SHEPHARD",
                "MALINOIS SHEPHARD CROSS",
                "MALTALIER",
                "MALTALIER CROSS",
                "MALTESE",
                "MALTESE CROSS",
                "MANCHESTER TERRIER",
                "MANCHESTER TERRIER CROSS",
                "MAREMMA SHEEPDOG",
                "MAREMMA SHEEPDOG CROSS",
                "MASTIFF",
                "MASTIFF CROSS",
                "MEXICAN HAIRLESS",
                "MEXICAN HAIRLESS CROSS",
                "MINIATURE BULL TERRIER",
                "MINIATURE BULL TERRIER CROSS",
                "MINIATURE DACHSHUND",
                "MINIATURE DACHSHUND - SMOOTH HAIRED",
                "MINIATURE DACHSHUND - WIRE HAIRED",
                "MINIATURE DACHSHUND - WIRE HAIRED CROSS",
                "MINIATURE DACHSHUND CROSS",
                "MINIATURE DACHSHUND CROSS CROSS",
                "MINIATURE DACHSHUND- LONG HAIRED",
                "MINIATURE DACHSHUND- LONG HAIRED CROSS",
                "MINIATURE DOBERMAN",
                "MINIATURE DOBERMAN CROSS",
                "MINIATURE FOX TERRIER",
                "MINIATURE FOX TERRIER CROSS",
                "MINIATURE MALTESE",
                "MINIATURE MALTESE CROSS",
                "MINIATURE PINSCHER",
                "MINIATURE PINSCHER CROSS",
                "MINIATURE POODLE",
                "MINIATURE POODLE CROSS",
                "MINIATURE SCHNAUZER",
                "MINIATURE SCHNAUZER CROSS",
                "MINIATURE TOY POM",
                "MINIATURE TOY POM CROSS",
                "MUNSTER",
                "MUNSTER CROSS",
                "MUNSTERLANDER - LARGE",
                "MUNSTERLANDER - LARGE CROSS",
                "NEOPOLITAN MASTIFF",
                "NEOPOLITAN MASTIFF CROSS",
                "NEW GUINEA SINGING DOG",
                "NEW GUINEA SINGING DOG CROSS",
                "NEW ZEALAND HUNTAWAY",
                "NEW ZEALAND HUNTAWAY CROSS",
                "NEWFOUNDLAND",
                "NEWFOUNDLAND CROSS",
                "NORFOLK TERRIER",
                "NORFOLK TERRIER CROSS",
                "NORWEGIAN ELK HOUND",
                "NORWEGIAN ELK HOUND CROSS",
                "NORWEGIAN HOUND",
                "NORWEGIAN HOUND CROSS",
                "NORWICH TERRIER",
                "NORWICH TERRIER CROSS",
                "NOVA SCOTIA DUCK TOLLING RETRIEVER",
                "NOVA SCOTIA DUCK TOLLING RETRIEVER CROSS",
                "OLD ENGLISH SHEEPDOG",
                "OLD ENGLISH SHEEPDOG CROSS",
                "OTTERHOUND",
                "OTTERHOUND CROSS",
                "PAPILLON",
                "PAPILLON CROSS",
                "PARSON JACK RUSSELL TERRIER",
                "PARSON JACK RUSSELL TERRIER CROSS",
                "PATTERDALE TERRIER",
                "PATTERDALE TERRIER CROSS",
                "PEKINGESE",
                "PEKINGESE CROSS",
                "PETIT BASSET GRIFFON VENDEEN",
                "PETIT BASSET GRIFFON VENDEEN CROSS",
                "PHARAOH HOUND",
                "PHARAOH HOUND CROSS",
                "PINSCHER",
                "PINSCHER CROSS",
                "PLOTT HOUND",
                "PLOTT HOUND CROSS",
                "POLISH LOWLAND SHEEPDOG",
                "POLISH LOWLAND SHEEPDOG CROSS",
                "POMERANIAN",
                "POMERANIAN CROSS",
                "POODLE - STANDARD",
                "POODLE - STANDARD CROSS",
                "POODLE - TOY",
                "POODLE - TOY CROSS",
                "PORTUGUESE PODENGO",
                "PORTUGUESE PODENGO LARGE",
                "PORTUGUESE PODENGO LARGE CROSS",
                "PORTUGUESE PODENGO MEDIUM",
                "PORTUGUESE PODENGO MEDIUM CROSS",
                "PORTUGUESE PODENGO SMALL",
                "PORTUGUESE PODENGO SMALL CROSS",
                "PORTUGUESE WATER DOG",
                "PORTUGUESE WATER DOG CROSS",
                "PRAGUE RATTER",
                "PRAGUE RATTER CROSS",
                "PUG",
                "PUG CROSS",
                "PULI",
                "PULI CROSS",
                "PYRENEAN MOUNTAIN DOG",
                "PYRENEAN MOUNTAIN DOG CROSS",
                "RAT TERRIER",
                "RAT TERRIER CROSS",
                "RHODESIAN RIDGEBACK",
                "RHODESIAN RIDGEBACK CROSS",
                "ROTTWEILER",
                "ROTTWEILER CROSS",
                "SALUKI",
                "SALUKI CROSS",
                "SAMOYED",
                "SAMOYED CROSS",
                "SARPLANINAC",
                "SARPLANINAC CROSS",
                "SCHIPPERKE",
                "SCHIPPERKE CROSS",
                "SCHNAUZER GIANT",
                "SCHNAUZER GIANT CROSS",
                "SCHNAUZER STANDARD",
                "SCHNAUZER STANDARD CROSS",
                "SCHNOODLE",
                "SCHNOODLE CROSS",
                "SCOTCH COLLIE",
                "SCOTCH COLLIE CROSS",
                "SCOTTISH TERRIER",
                "SCOTTISH TERRIER CROSS",
                "SEALYHAM TERRIER",
                "SEALYHAM TERRIER CROSS",
                "SHAR-PEI",
                "SHAR-PEI CROSS",
                "SHETLAND SHEEPDOG",
                "SHETLAND SHEEPDOG CROSS",
                "SHIBA INU",
                "SHIBA INU CROSS",
                "SHIH TZU",
                "SHIH TZU CROSS",
                "SHILOH SHEPHERD",
                "SHILOH SHEPHERD CROSS",
                "SIBERIAN HUSKY",
                "SIBERIAN HUSKY CROSS",
                "SKYE TERRIER",
                "SKYE TERRIER CROSS",
                "SMITHFIELD",
                "SMITHFIELD CROSS",
                "SOFT COATED WHEATEN TERRIER",
                "SOFT COATED WHEATEN TERRIER CROSS",
                "SPANADOR",
                "SPANADOR CROSS",
                "SPANISH WATER DOG",
                "SPANISH WATER DOG CROSS",
                "SPITZ MITTEL",
                "SPITZ MITTEL CROSS",
                "SPOODLE",
                "SPOODLE CROSS",
                "SPRINGER SPANIEL",
                "SPRINGER SPANIEL CROSS",
                "ST BERNARD",
                "ST BERNARD CROSS",
                "STAFFORDSHIRE BULL TERRIER",
                "STAFFORDSHIRE BULL TERRIER CROSS",
                "STAGHOUND",
                "STAGHOUND CROSS",
                "STUMPY TAIL CATTLE DOG",
                "STUMPY TAIL CATTLE DOG CROSS",
                "SUSSEX SPANIEL",
                "SUSSEX SPANIEL CROSS",
                "SWEDISH VALLHUND",
                "SWEDISH VALLHUND CROSS",
                "SWISS MOUNTAIN DOG",
                "SWISS MOUNTAIN DOG CROSS",
                "SWISS SHEPHERD",
                "SWISS SHEPHERD CROSS",
                "TENTERFIELD TERRIER",
                "TENTERFIELD TERRIER CROSS",
                "TIBETAN MASTIFF",
                "TIBETAN MASTIFF CROSS",
                "TIBETAN SPANIEL",
                "TIBETAN SPANIEL CROSS",
                "TIBETAN TERRIER",
                "TIBETAN TERRIER CROSS",
                "TIMBER SHEPHERD",
                "TIMBER SHEPHERD CROSS",
                "TURKISH KANGAL",
                "TURKISH KANGAL CROSS",
                "WEIMARANER",
                "WEIMARANER CROSS",
                "WELSH CORGI - CARDIGAN",
                "WELSH CORGI - CARDIGAN CROSS",
                "WELSH CORGI - PEMBROKE",
                "WELSH CORGI - PEMBROKE CROSS",
                "WELSH SPRINGER SPANIEL",
                "WELSH SPRINGER SPANIEL CROSS",
                "WELSH TERRIER",
                "WELSH TERRIER CROSS",
                "WEST HIGHLAND WHITE TERRIER",
                "WEST HIGHLAND WHITE TERRIER CROSS",
                "WHIPPET",
                "WHIPPET CROSS",
                "WIRE-HAIRED POINTING GRIFFON",
                "WIRE-HAIRED POINTING GRIFFON CROSS",
                "WIRE-HAIRED TERRIER",
                "WIRE-HAIRED TERRIER CROSS",
                "WOLFHOUND",
                "WOLFHOUND CROSS",
                "YORKSHIRE TERRIER",
                "YORKSHIRE TERRIER CROSS",
                "YUGOSLAVIAN MOUNTAIN HOUND",
                "YUGOSLAVIAN MOUNTAIN HOUND CROSS"
            };

            return dogOptions.OrderBy(t => Guid.NewGuid()).First();
        }
    }
}

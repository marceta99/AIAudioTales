using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;

namespace Kumadio.Infrastructure.Data.Seed.BooksPictures
{
    public class DbSeederService
    {
        private readonly AppDbContext _dbContext;

        public DbSeederService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            // Seed the database with initial data
            if (!_dbContext.Users.Any())
            {
                var usersInitialData = new List<User>
                {
                    new User
                    {
                       Email = "kumadio@gmail.com",
                       PasswordHash = new byte[]{},
                       PasswordSalt = new byte[] {},
                       Role = Role.CREATOR,
                       FirstName = "Kumadio",
                       LastName = "Kumadic"
                    }
                };
                _dbContext.Users.AddRange(usersInitialData);
                _dbContext.SaveChanges();
            }


            if (!_dbContext.BookCategories.Any())
            {
                var categoryInitialData = new List<Category>
                {
                    new Category
                    {
                        Id = 1,
                        CategoryName = "BedTime",
                        Description = "This category provides kids with best bed time books"
                    },
                    new Category
                    {   Id = 2,
                        CategoryName = "History",
                        Description = "This category provides kids with best History books"
                    },
                    new Category
                    {   Id = 3,
                        CategoryName = "Math",
                        Description = "This category provides kids with best Math books"
                    },
                    new Category
                    {   Id = 4,
                        CategoryName = "Geography",
                        Description = "This category provides kids with best Geography books"
                    },
                    new Category
                    {   Id = 5,
                        CategoryName = "Nature",
                        Description = "This category provides kids with best Nature books"
                    },
                    new Category
                    {   Id = 6,
                        CategoryName = "Trending",
                        Description = "This category provides kids with best Trending books"
                    },
                    new Category
                    {   Id = 7,
                        CategoryName = "Recommended",
                        Description = "This category provides kids with best Recommended books"
                    },
                };
                _dbContext.BookCategories.AddRange(categoryInitialData);
                _dbContext.SaveChanges();
            }

            var creator = _dbContext.Users.Where(u => u.Email == "kumadio@gmail.com").FirstOrDefault();

            if (creator != null && !_dbContext.Books.Any())
            {
                // Seed the database with initial data, only changing Title and Description to Serbian
                var initialDataSrb = new List<Book>
{
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Priče sa Male Farme",
        Description="Mala farma ispunjena je mudrim životinjama koje decu vode kroz pustolovine prirode i prijateljstva.",
        ImageURL = "https://picsum.photos/784",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Harmonija Nebeskih Polja",
        Description="U visinama zvezdanog neba, ptice pevaju o jedinstvu prirode i zvucima vetra koji grle nepregledne predele.",
        ImageURL = "https://picsum.photos/321",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Odjek Zelenih Planina",
        Description="U podnožju visokih planina, devojčica otkriva skrivena sela dok je žubor potoka prati na putu znanja.",
        ImageURL = "https://picsum.photos/173",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Nasleđe Malog Matematičara",
        Description="Dečak pronalazi magičnu tablicu množenja koja otkriva tajne brojeva, pretvarajući svaki zadatak u igru.",
        ImageURL = "https://picsum.photos/344",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Skrivene Priče Starih Dvoraca",
        Description="U senci drevnih zamkova, hrabri mali istoričar otkriva legende o vitezovima i kraljevima prošlih vremena.",
        ImageURL = "https://picsum.photos/595",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Uspavanka Zlatnih Zvezda",
        Description="Dok noć pada, zvezde tkaju nežnu mrežu snova, donoseći miran san svakom detetu.",
        ImageURL = "https://picsum.photos/436",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Mape Skrivenih Kontinenata",
        Description="Listajući stare karte, devojčica uči o pustinjama, morima i polarnim predelima dalekih krajeva.",
        ImageURL = "https://picsum.photos/292",
    },
    new Book
    {
        CreatorId = creator.Id,
        CategoryId=5,
        Title="Tajna Šumske Čistine",
        Description="Usred guste šume nalazi se čistina gde se životinje okupljaju i govore o lepotama prirode.",
        ImageURL = "https://picsum.photos/589",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Cvetna Reka",
        Description="Potok kroz livade nosi latice cveća, raznoseći priče od jedne obale do druge.",
        ImageURL = "https://picsum.photos/583",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Vetrovita Dolina",
        Description="U dolini gde vetar pevuši, deca otkrivaju tajne drveća dok mesec nad njima sija.",
        ImageURL = "https://picsum.photos/939",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Ptičji Hor",
        Description="U svitanje se čuje hor ptica, svaka pesma priča o prijateljstvu i novom početku.",
        ImageURL = "https://picsum.photos/345",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Mali Brojevi, Velike Tajne",
        Description="Dečak otkriva da sabiranje nije samo račun, već put kroz svet ideja i oblika.",
        ImageURL = "https://picsum.photos/968",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Geometrijski Vitezovi",
        Description="U magičnoj knjizi oblici oživljavaju kao vitezovi što čuvaju harmoniju brojeva.",
        ImageURL = "https://picsum.photos/880",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Priča o Deljenju",
        Description="Dve sestre dele jabuku i shvataju da deljenje donosi slast i radost zajedništva.",
        ImageURL = "https://picsum.photos/557",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Zagonetka Kruga",
        Description="Jedan krug krije savršenstvo, učeći devojčicu o beskrajnoj liniji bez početka i kraja.",
        ImageURL = "https://picsum.photos/318",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Most Preko Sveta",
        Description="Zamišljeni most spaja različite zemlje i kulture, uvodeći dete u čudesni svet geografije.",
        ImageURL = "https://picsum.photos/262",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Zemlja Pet Jezera",
        Description="U zemlji pet jezera, dečak uči o ribarima, pticama i oblacima koji plove nad vodom.",
        ImageURL = "https://picsum.photos/398",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Planinski Putnici",
        Description="Na strmim stazama planine, deca sreću nomade i uče imena dalekih vrhova.",
        ImageURL = "https://picsum.photos/738",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Morska Kapija",
        Description="Stara luka je kapija u svet, gde brodovi donose začine i priče udaljenih naroda.",
        ImageURL = "https://picsum.photos/643",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Drevni Zapisi Hroničara",
        Description="Mali istoričar otkriva prašnjave svitke što čuvaju priče o kraljevima i bitkama prošlih vekova.",
        ImageURL = "https://picsum.photos/385",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Štit Od Legendi",
        Description="Iza starih zidina, legende o herojima i vitezovima sjaje poput dragulja istorije.",
        ImageURL = "https://picsum.photos/347",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Senke Prošlosti",
        Description="Na starom trgu, glas guslara čuva uspomene na davne bitke i zaboravljena blaga.",
        ImageURL = "https://picsum.photos/946",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Kraljičin Dar",
        Description="Kraljica je ostavila dragoceni dar narodu, a deca vekovima kasnije otkrivaju njegovu tajnu.",
        ImageURL = "https://picsum.photos/201",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Zvezdani San",
        Description="Dok se zvezde skupljaju iznad krova, dete tone u san ispunjen nežnim melodijama.",
        ImageURL = "https://picsum.photos/606",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Mjesečeva Ljuljaška",
        Description="Mesec postaje ljuljaška koja njiše dete kroz polja mirnih snova.",
        ImageURL = "https://picsum.photos/209",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Uspavanka Žubora",
        Description="Žubor potoka pretvara se u uspavanku koja šapuće o lepoti noći.",
        ImageURL = "https://picsum.photos/549",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Tišina Lanenih Polja",
        Description="Lanenih polja tiho plešu na vetru, darujući deci mir i blag san.",
        ImageURL = "https://picsum.photos/592",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Noćni Fenjer",
        Description="Mali fenjer na prozoru čuva topli sjaj koji prati dete do lepih snova.",
        ImageURL = "https://picsum.photos/624",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Miris Meda",
        Description="U bašti lipa pčele stvaraju med, a noćni vazduh ispunjen je slatkim mirom.",
        ImageURL = "https://picsum.photos/252",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Beli Lotos",
        Description="Na mirnoj vodi cveta beli lotos, dok zrikavci tiho pevaju uspavanku.",
        ImageURL = "https://picsum.photos/840",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Gnezdo Tišine",
        Description="Ptica gradi gnezdo na starom hrastu, čuvajući tišinu pred svitanje.",
        ImageURL = "https://picsum.photos/815",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Cifre Na Mesecu",
        Description="Mesec skriva cifre u svojim senkama, podučavajući dete čaroliji brojeva.",
        ImageURL = "https://picsum.photos/176",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Skakutavi Sabirač",
        Description="Zec koji sabira mrkve pokazuje da je matematika vesela i puna igre.",
        ImageURL = "https://picsum.photos/621",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Zvezdana Jednačina",
        Description="Zvezde na nebu tvore savršenu jednačinu, darujući miran san i tišinu.",
        ImageURL = "https://picsum.photos/849",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Šapat Livade",
        Description="Livada šapuće o leptirima i povetarcima, darujući mir svima koji slušaju.",
        ImageURL = "https://picsum.photos/990",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Ogledalo Planinskog Jezerca",
        Description="U bistroj vodi jezera planine se ogledaju, dok ribe pričaju o skladnom životu.",
        ImageURL = "https://picsum.photos/265",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Put Soli",
        Description="Karavan preko slanih ravnica nosi priče dalekih naroda i običaja.",
        ImageURL = "https://picsum.photos/546",
    },
    new Book
    {
        CreatorId = creator.Id,
        CategoryId=3,
        Title="Magični Sabirak",
        Description="Dečak nalazi magični prah koji, posut po brojevima, pretvara zadatke u igru.",
        ImageURL = "https://picsum.photos/462",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Oružje Zaboravljenih Junaka",
        Description="Stari mač u muzeju pripoveda legende o hrabrim junacima i njihovim pobedama.",
        ImageURL = "https://picsum.photos/922",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Pospana Čaršija",
        Description="U tihoj čaršiji lampe se gase, a mesečina daruje san svima koji traže mir.",
        ImageURL = "https://picsum.photos/134",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Horizont Ljubičastih Polja",
        Description="Lavandina polja na horizontu pričaju o mirisu i miru dalekih krajeva.",
        ImageURL = "https://picsum.photos/535",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Izvor Šumskih Vila",
        Description="Vile se okupljaju oko izvora, pripovedajući o suncu, kiši i večnom krugu života.",
        ImageURL = "https://picsum.photos/422",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Kapi Jutarnje Rose",
        Description="Rosa na listovima čuva priče noći koje ptice prepevavaju u zoru.",
        ImageURL = "https://picsum.photos/630",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Šum Breza",
        Description="Brezik tiho šapuće dok vetar nosi listove kao stranice stare priče.",
        ImageURL = "https://picsum.photos/257",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=5,
        Title="Tihi Proplanak",
        Description="Na tihom proplanku srne i zečevi dele priče o poverenju i prijateljstvu.",
        ImageURL = "https://picsum.photos/212",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Brojčani Labirint",
        Description="Dečak traži izlaz iz lavirinta gde zidovi nose brojeve umesto kamenja.",
        ImageURL = "https://picsum.photos/613",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Magija Sabiranja",
        Description="Kada se dva broja zagrle, postaju veći i srećniji, stvarajući čuda sabiranja.",
        ImageURL = "https://picsum.photos/991",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Mistični Delilac",
        Description="Čarobnjak deli jabuke svima jednako, učeći nas pravdi i ravnoteži.",
        ImageURL = "https://picsum.photos/394",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=3,
        Title="Zvezdani Niz",
        Description="Zvezde na nebu tvore niz brojeva koji čuva tajnu beskraja.",
        ImageURL = "https://picsum.photos/417",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Obale Tišine",
        Description="More izbacuje školjke koje šapuću o zemljama iza dalekog horizonta.",
        ImageURL = "https://picsum.photos/200",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Zemlja Dalekih Kula",
        Description="Na brdima stoje kule koje pamte priče o plemićima i starim običajima.",
        ImageURL = "https://picsum.photos/546",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Pesak i Vetar",
        Description="U pustinji pesak i vetar crtaju mape sveta bez granica.",
        ImageURL = "https://picsum.photos/495",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=4,
        Title="Ledena Čarolija",
        Description="Na dalekom severu led priča o medvedima, fokama i hrabrosti opstanka.",
        ImageURL = "https://picsum.photos/421",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Tragovi Minulih Dana",
        Description="Na starom kaldrmisanom putu čuju se koraci istorije koja nikad ne spava.",
        ImageURL = "https://picsum.photos/975",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Đerdan Herbarijuma",
        Description="Stare knjige čuvaju suvo cveće, uspomene na prohujala proleća.",
        ImageURL = "https://picsum.photos/785",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Sećanja Urezana U Stenu",
        Description="Stene na liticama pamte zvuke truba i pesme starih ratnika.",
        ImageURL = "https://picsum.photos/578",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=2,
        Title="Pečat Vremena",
        Description="Spomenici na trgu pečate epohe, sećajući se minulih heroja.",
        ImageURL = "https://picsum.photos/978",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Zlatni Ugaonik",
        Description="U ćošku sobe zlatni ugao čuva priče sanjara pretvarajući ih u uspavanke.",
        ImageURL = "https://picsum.photos/515",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Noćni Ćup",
        Description="Ćup kraj kreveta puni se mesečinom i snovima toplim poput meda.",
        ImageURL = "https://picsum.photos/231",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Mehuri Sna",
        Description="Mehuri od sapunice lebde sobom, noseći dete ka nežnim snovima.",
        ImageURL = "https://picsum.photos/314",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Svileni Prekrivač",
        Description="Svileni prekrivač štiti dete od briga, vodeći ga u topao i tih san.",
        ImageURL = "https://picsum.photos/252",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Mesečev Grad",
        Description="Grad obasjan mesečinom priča tihe priče o dobroti i snovima.",
        ImageURL = "https://picsum.photos/363",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Rasadnik Zvezda",
        Description="Noćni vrt u kome zvezde niču kao seme nade i mira.",
        ImageURL = "https://picsum.photos/262",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Miris Šumskog Sna",
        Description="Miris šume ulazi kroz prozor, grleći dete nežnim zagrljajem tišine.",
        ImageURL = "https://picsum.photos/628",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Tiha Luka",
        Description="Brodovi ćute u luci, a mesec uspavljuje more mirnim tonovima.",
        ImageURL = "https://picsum.photos/765",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Brojčana Uspavanka",
        Description="Svaka cifra šapuće priču, pretvarajući zadatke u nežne pesme sna.",
        ImageURL = "https://picsum.photos/263",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Skok Preko Brojeva",
        Description="Kao žaba na listu, dete skače preko brojeva u svet mirnog sna.",
        ImageURL = "https://picsum.photos/799",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=1,
        Title="Cvet Jednačine",
        Description="Jednačina cveta u bašti brojeva, darujući mudrost i mir.",
        ImageURL = "https://picsum.photos/735",
    },

    // Recommended (7) and others below, same logic: just change Title and Description
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Krug Planeta",
        Description="Preporučena priča o detetu koje crta planete u krug, otkrivajući beskraj svemira.",
        ImageURL = "https://picsum.photos/359",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Most Između Kultura",
        Description="Knjiga preporučuje putovanje preko mosta koji spaja pesnike i slikare sveta.",
        ImageURL = "https://picsum.photos/671",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Peščana Uspomena",
        Description="Preporučena priča o nomadu i pesku koji pamte svaki korak kroz vreme.",
        ImageURL = "https://picsum.photos/215",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Dolina Šapata",
        Description="Glas vetra u dolini preporučuje da upoznamo daleke predele i jezike.",
        ImageURL = "https://picsum.photos/562",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Izvor Izgubljene Reke",
        Description="Preporuka da pronađemo izvor reke koja je nestala s drevnih mapa.",
        ImageURL = "https://picsum.photos/230",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Trag Preko Okeana",
        Description="Preporučena avantura brodom koji spaja kontinente pesmom i mirom.",
        ImageURL = "https://picsum.photos/969",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporučena Uspavanka",
        Description="Sanjari kažu da ova priča donosi miran san i nežne note noći.",
        ImageURL = "https://picsum.photos/369",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Mesečeve Merdevine",
        Description="Preporučuje se priča o merdevinama do meseca, gde snovi plešu.",
        ImageURL = "https://picsum.photos/653",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Zamak Od Snoviđenja",
        Description="Preporučena priča o zamku od snova gde deca nalaze spokoj i tišinu.",
        ImageURL = "https://picsum.photos/957",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Oblak Jastuka",
        Description="Mekani oblak se preporučuje kao najudobniji jastuk za nežan počinak.",
        ImageURL = "https://picsum.photos/959",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Sidnejev Izvor",
        Description="Preporučena priča o Sidneju koji otkriva skriveni izvor radosti u srcu šume.",
        ImageURL = "https://picsum.photos/363",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporuka Latičnog Bala",
        Description="Cveće se okuplja na plesu, preporučujući sklad i lepotu prirode.",
        ImageURL = "https://picsum.photos/690",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Šuma Preporuka",
        Description="Svaki list u šumi preporučuje mir, svaka grana priča o skladu sa svetom.",
        ImageURL = "https://picsum.photos/206",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Bele Breze",
        Description="Breze preporučuju čistu tišinu i šapat vetra kao najlepšu muziku.",
        ImageURL = "https://picsum.photos/209",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporučena Cifra",
        Description="Jedna cifra preporučuje da zavolimo učenje brojeva i igru.",
        ImageURL = "https://picsum.photos/389",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Savršeni Par Brojeva",
        Description="Dva broja se spajaju, preporučujući radost zajedničkog rešavanja zadataka.",
        ImageURL = "https://picsum.photos/990",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Most Preko Jednačina",
        Description="Preporučuje se priča o mostu koji povezivanjem brojeva donosi razumevanje.",
        ImageURL = "https://picsum.photos/469",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporuka Poljskog Puta",
        Description="Poljski put predlaže skromnost i zahvalnost za plodove zemlje.",
        ImageURL = "https://picsum.photos/566",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Zvuci Nebeskog Hoda",
        Description="Melodije vetra i zvezda usklađuju srce i misli, preporučujući unutrašnji mir.",
        ImageURL = "https://picsum.photos/770",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Odjek Preporuke",
        Description="Reči odzvanjaju u planinama, prenoseći mudrost predaka kao preporuku.",
        ImageURL = "https://picsum.photos/870",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Nasleđe Preporuke",
        Description="Alhemičar ostavlja preporuku da mudrost pronađemo i u zrncu peska.",
        ImageURL = "https://picsum.photos/780",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporuka Starih Zidina",
        Description="Zidine starog grada šapuću da slušamo istoriju i učimo iz nje.",
        ImageURL = "https://picsum.photos/861",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporuka Tajnog Mehanizma",
        Description="U staroj kuli mehanizam savetuje strpljenje i potragu za smislom.",
        ImageURL = "https://picsum.photos/632",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporuka Najboljeg",
        Description="Knjiga savetuje da biramo dobrotu i ljubav kao najjaču snagu.",
        ImageURL = "https://picsum.photos/943",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporuka Veličanstvenog",
        Description="Priča predlaže da veličinu tražimo u skromnim delima svakodnevice.",
        ImageURL = "https://picsum.photos/718",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporuka Šumskog Gaja",
        Description="Gaj šapuće da slušamo lišće i nalazimo mir u tišini.",
        ImageURL = "https://picsum.photos/241",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=7,
        Title="Preporučena Lepa Trava",
        Description="Svaka vlas trave savetuje blagost i mekoću naših koraka.",
        ImageURL = "https://picsum.photos/622",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Tehnološko Proleće",
        Description="U vremenu trendova priroda i tehnika plešu zajedno, spajajući stara i nova čuda.",
        ImageURL = "https://picsum.photos/207",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Digitalni Brojevi",
        Description="Brojevi na ekranima postaju prijatelji dece, učeći ih matematici kroz igru.",
        ImageURL = "https://picsum.photos/149",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Aplikacija za Sabiranje",
        Description="Trend je koristiti aplikacije da sabiranjem gradimo mostove među brojevima.",
        ImageURL = "https://picsum.photos/259",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Kodirani Brojevi",
        Description="Elektronski uređaji šapuću brojevne kodove, pretvarajući učenje u igru.",
        ImageURL = "https://picsum.photos/851",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Vreme i Brojčanik",
        Description="Sat deli dan na jednake delove, učeći decu važnosti svakog trenutka.",
        ImageURL = "https://picsum.photos/494",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Virtuelne Granice",
        Description="Mape na ekranu vode decu kroz gradove i reke, trend modernog putovanja.",
        ImageURL = "https://picsum.photos/546",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Interaktivni Globus",
        Description="Deca vrte digitalni globus, učeći o kontinentima dodirom prsta.",
        ImageURL = "https://picsum.photos/174",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Online Putnik",
        Description="Putovati svetom kroz ekran postaje trend, spajajući blizinu i daljinu.",
        ImageURL = "https://picsum.photos/433",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Snimci iz Vazduha",
        Description="Dronovi slikaju svet odozgo, trend je videti planetu iz ptičje perspektive.",
        ImageURL = "https://picsum.photos/934",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Pametne Mape",
        Description="Mape govore imena gradova, trend je upoznati svet klikom.",
        ImageURL = "https://picsum.photos/813",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Digitalni Čarobnjak",
        Description="Aplikacija vodi preko planina i dolina, trend interaktivnog učenja.",
        ImageURL = "https://picsum.photos/948",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Virtuelna Sahara",
        Description="VR naočare oživljavaju pustinju, trend osećanja dalekih predela iz sobe.",
        ImageURL = "https://picsum.photos/440",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Elektronski Kompas",
        Description="Trend je pratiti sever elektronski, šireći znanje o geografiji.",
        ImageURL = "https://picsum.photos/556",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="E-Uspavanka",
        Description="Trend je slušati uspavanku preko zvučnika, nalazeći mir u novim tehnologijama.",
        ImageURL = "https://picsum.photos/560",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="LED Zvezdice",
        Description="Na zidu sijaju LED zvezdice, moderan trend koji donosi spokoj noći.",
        ImageURL = "https://picsum.photos/415",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Aplikacija za San",
        Description="Trend je pratiti san kroz aplikaciju, tražeći savršen odmor.",
        ImageURL = "https://picsum.photos/795",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Digitalna Harfa",
        Description="Harfa svira tiho preko tableta, trend mirnog tona pred spavanje.",
        ImageURL = "https://picsum.photos/161",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Hologramski Prijatelj",
        Description="Trend je stvoriti hologramskog prijatelja koji priča pred san.",
        ImageURL = "https://picsum.photos/671",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Eko-Senzori",
        Description="Trend su senzori u bašti koji govore kada cveće žedni, čuvajući prirodu.",
        ImageURL = "https://picsum.photos/927",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Solarny Pašnjak",
        Description="Trend održivosti: solarna energija čuva zeleni pašnjak za životinje.",
        ImageURL = "https://picsum.photos/653",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Digitalni Čvorci",
        Description="Trend je slušati čvorke preko aplikacije, spajajući grad i prirodu.",
        ImageURL = "https://picsum.photos/238",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Interaktivni Broj",
        Description="Trend: dodirni ekran, broj propeva priču o sebi.",
        ImageURL = "https://picsum.photos/880",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Algoritamska Igra",
        Description="Trend je rešavati zadatke kroz aplikacije koje računaju umesto nas.",
        ImageURL = "https://picsum.photos/153",
    },
    new Book
    {
        CreatorId = creator.Id,
        Price = 6,
        CategoryId=6,
        Title="Virtualna Tabla",
        Description="Trend je pisati cifre na virtuelnoj tabli, bez granica i krede.",
        ImageURL = "https://picsum.photos/834",
    },
};

                var initialDataEng = new List<Book>
                {
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "Stories from the Little Farm",
                        Description = "A little farm filled with wise animals who guide children through adventures in nature and friendship.",
                        ImageURL = "https://picsum.photos/200"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "Harmony of the Celestial Fields",
                        Description = "High in the starry sky, birds sing of nature's unity and the sounds of the wind embracing vast landscapes.",
                        ImageURL = "https://picsum.photos/201"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 4,
                        Title = "Echo of the Green Mountains",
                        Description = "At the foot of tall mountains, a girl discovers hidden villages as the murmuring stream accompanies her path to knowledge.",
                        ImageURL = "https://picsum.photos/202"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 3,
                        Title = "Legacy of the Little Mathematician",
                        Description = "A boy finds a magical multiplication table revealing the secrets of numbers, turning every task into a game.",
                        ImageURL = "https://picsum.photos/203"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 2,
                        Title = "Hidden Tales of Ancient Castles",
                        Description = "In the shadows of ancient castles, a brave little historian discovers legends of knights and kings of the past.",
                        ImageURL = "https://picsum.photos/204"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 1,
                        Title = "Lullaby of Golden Stars",
                        Description = "As night falls, stars weave a gentle web of dreams, bringing peaceful sleep to every child.",
                        ImageURL = "https://picsum.photos/205"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 4,
                        Title = "Maps of Hidden Continents",
                        Description = "Flipping through old maps, a girl learns about deserts, seas, and polar regions of distant lands.",
                        ImageURL = "https://picsum.photos/206"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "Secret of the Forest Clearing",
                        Description = "In the heart of the dense forest, there is a clearing where animals gather and speak of nature's beauty.",
                        ImageURL = "https://picsum.photos/207"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "Floral River",
                        Description = "A stream flowing through meadows carries flower petals, spreading stories from one bank to the other.",
                        ImageURL = "https://picsum.photos/208"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "Windy Valley",
                        Description = "In a valley where the wind hums, children uncover the secrets of trees while the moon shines above.",
                        ImageURL = "https://picsum.photos/209"
                    },

                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 1,
                        Title = "The Whispering Willow",
                        Description = "A curious child discovers the secrets hidden in an ancient willow tree.",
                        ImageURL = "https://picsum.photos/210"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 2,
                        Title = "The Rainbow Carousel",
                        Description = "Join the magical carousel that travels through colors and dreams.",
                        ImageURL = "https://picsum.photos/211"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 3,
                        Title = "Starlight Explorers",
                        Description = "A group of friends builds a spaceship out of cardboard and visits the stars.",
                        ImageURL = "https://picsum.photos/212"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 4,
                        Title = "The Secret Garden Key",
                        Description = "Emily finds a key that opens a hidden garden full of talking flowers.",
                        ImageURL = "https://picsum.photos/213"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "The Cloud Painter",
                        Description = "A young artist learns to paint shapes in the sky that come to life.",
                        ImageURL = "https://picsum.photos/214"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 6,
                        Title = "Robot Buddy",
                        Description = "Leo builds a little robot who becomes his best friend.",
                        ImageURL = "https://picsum.photos/215"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 7,
                        Title = "Forest of Giggles",
                        Description = "In a forest where trees giggle, Mia discovers laughter is magic.",
                        ImageURL = "https://picsum.photos/216"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 1,
                        Title = "The Midnight Bakery",
                        Description = "Every night, a bakery opens only under the moonlight, baking dream-flavored treats.",
                        ImageURL = "https://picsum.photos/217"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 2,
                        Title = "Dragonfly Daydream",
                        Description = "A dragonfly takes a tired child on a gentle flight over flower fields.",
                        ImageURL = "https://picsum.photos/218"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 3,
                        Title = "The Pocket-Sized Pirate",
                        Description = "A brave little pirate in your pocket brings big ocean adventures.",
                        ImageURL = "https://picsum.photos/219"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 4,
                        Title = "Sock Puppet Theater",
                        Description = "Tommy’s sock puppets come alive and put on a grand performance.",
                        ImageURL = "https://picsum.photos/220"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "The Magic Compass",
                        Description = "A compass that points to kindness leads Ava on a heartwarming journey.",
                        ImageURL = "https://picsum.photos/221"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 6,
                        Title = "Moonbeam Music",
                        Description = "Children discover melodies hidden in moonbeams on a starry night.",
                        ImageURL = "https://picsum.photos/222"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 7,
                        Title = "The Tiny Train That Could",
                        Description = "A small toy train sets off on a big adventure around the playroom.",
                        ImageURL = "https://picsum.photos/223"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 1,
                        Title = "Bubbles in the Breeze",
                        Description = "Soap bubbles carry laughter and tiny wishes through the summer air.",
                        ImageURL = "https://picsum.photos/224"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 2,
                        Title = "The Snowflake Circus",
                        Description = "A circus of snowflakes performs dazzling acrobatics in winter skies.",
                        ImageURL = "https://picsum.photos/225"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 3,
                        Title = "The Wishing Well",
                        Description = "A hidden well grants wishes that teach gentle lessons of generosity.",
                        ImageURL = "https://picsum.photos/226"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 4,
                        Title = "Pirate’s Paper Hat",
                        Description = "A paper hat transforms a child into captain of a paper-ship exploring puddles.",
                        ImageURL = "https://picsum.photos/227"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 5,
                        Title = "The Garden of Dreams",
                        Description = "Flowers in this garden bloom only when children share their dreams.",
                        ImageURL = "https://picsum.photos/228"
                    },
                    new Book
                    {
                        CreatorId = creator.Id,
                        Price = 6,
                        CategoryId = 6,
                        Title = "The Little Lantern",
                        Description = "A tiny lantern lights the way for nighttime adventures in a dollhouse village.",
                        ImageURL = "https://picsum.photos/229"
                    }
                };

                _dbContext.Books.AddRange(initialDataSrb);
                _dbContext.SaveChanges();
            }
        }


    }
}

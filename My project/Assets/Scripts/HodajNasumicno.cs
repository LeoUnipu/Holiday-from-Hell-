using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HodajNasumicno : MonoBehaviour
{
    public enum SmjerGledanja
    {
        Automatski,
        Lijevo,
        Desno,
        Naprijed,
        Nazad
    }

    [System.Serializable]
    public class KorakRutine
    {
        [Header("Naziv koraka")]
        public string nazivKoraka;

        [Header("Mjesto dolaska")]
        public Transform mjestoDolaska;

        [Header("Smjer gledanja nakon dolaska")]
        [Tooltip(
            "Smjer u kojem NPC gleda čim dođe do mjesta. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaNakonDolaska =
            SmjerGledanja.Automatski;

        [Header("Smjer gledanja tijekom radnje")]
        [Tooltip(
            "Smjer u kojem NPC gleda tijekom izvođenja radnje. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaTijekomRadnje =
            SmjerGledanja.Automatski;

        [Header("Smjer gledanja nakon radnje")]
        [Tooltip(
            "Smjer u kojem se NPC okrene nakon završene radnje, " +
            "prije odlaska prema sljedećem mjestu."
        )]
        public SmjerGledanja smjerGledanjaNakonRadnje =
            SmjerGledanja.Automatski;

        [Header("Teleport između katova")]
        public bool teleportiraj = false;
        public Transform teleportIzlaz;

        [Header("Zvuk otvaranja vrata kod teleporta")]
        [Tooltip("Zvuk koji se reproducira nakon teleportiranja kroz vrata.")]
        public AudioClip zvukOtvaranjaVrata;

        [Range(0f, 1f)]
        public float glasnocaZvukaOtvaranjaVrata = 1f;

        [Header("Animacija radnje")]
        [Tooltip(
            "Naziv Trigger parametra za animaciju radnje. " +
            "Primjer: Drink. Ostavi prazno ako ovaj korak nema animaciju."
        )]
        public string triggerRadnje = "";

        [Tooltip(
            "Točan naziv Animator stanja radnje. " +
            "Primjer: Drink."
        )]
        public string stanjeRadnje = "";

        [Tooltip(
            "Najduže vrijeme čekanja završetka animacije radnje."
        )]
        public float maksimalnoTrajanjeAnimacijeRadnje = 10f;

        [Header("Zvuk animacije radnje")]
        [Tooltip("Zvuk koji se reproducira kada počne animacija radnje.")]
        public AudioClip zvukRadnje;

        [Range(0f, 1f)]
        public float glasnocaZvukaRadnje = 1f;

        [Header("Animacija dok nema zamke")]
        [Tooltip(
            "Animacija koja se izvršava kada zamka nije postavljena. " +
            "Ne izvršava se kada je zamka aktivna."
        )]
        public string triggerDokNemaZamke = "";

        [Tooltip(
            "Točan naziv Animator stanja koje se izvršava " +
            "kada zamka nije postavljena."
        )]
        public string stanjeDokNemaZamke = "";

        [Tooltip(
            "Najduže čekanje završetka animacije dok nema zamke."
        )]
        public float maksimalnoTrajanjeDokNemaZamke = 10f;

        [Header("Smjer gledanja tijekom animacije dok nema zamke")]
        [Tooltip(
            "Smjer u kojem NPC gleda tijekom animacije dok nema zamke. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaDokNemaZamke =
            SmjerGledanja.Automatski;

        [Header("Smjer gledanja nakon animacije dok nema zamke")]
        [Tooltip(
            "Smjer u kojem NPC gleda nakon što animacija dok nema zamke završi. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaNakonDokNemaZamke =
            SmjerGledanja.Automatski;

        [Header("Zvuk animacije dok nema zamke")]
        [Tooltip("Zvuk koji se reproducira kada počne animacija dok nema zamke.")]
        public AudioClip zvukDokNemaZamke;

        [Range(0f, 1f)]
        public float glasnocaZvukaDokNemaZamke = 1f;

        [Header("Zamka")]
        public GameObject aktivnaZamka;

        [Header("Efekt zamke")]
        [Tooltip(
            "Opcionalni Particle System koji se pokreće kada NPC aktivira zamku. " +
            "Ostavi prazno ako zamka nema efekt."
        )]
        public ParticleSystem efektZamke;

        [Tooltip("Koliko bodova igrač dobije za ovu zamku.")]
        public int bodoviZaZamku = 100;

        [Header("Reakcija")]
        [Tooltip("Naziv Trigger parametra u Animatoru.")]
        public string triggerReakcije = "Angry";

        [Tooltip("Točan naziv Animator stanja reakcije.")]
        public string stanjeReakcije = "Angry";

        [Tooltip(
            "Najduže čekanje završetka reakcije. " +
            "Služi kao zaštita ako naziv stanja nije dobro postavljen."
        )]
        public float maksimalnoTrajanjeReakcije = 10f;

        [Header("Smjer gledanja tijekom reakcije")]
        [Tooltip(
            "Smjer u kojem NPC gleda tijekom reakcije na zamku. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaTijekomReakcije =
            SmjerGledanja.Automatski;

        [Header("Smjer gledanja nakon reakcije")]
        [Tooltip(
            "Smjer u kojem NPC gleda nakon što reakcija završi. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaNakonReakcije =
            SmjerGledanja.Automatski;

        [Header("Zvuk reakcije")]
        [Tooltip("Zvuk prve reakcije na zamku, primjerice Fall.")]
        public AudioClip zvukReakcije;

        [Range(0f, 1f)]
        public float glasnocaZvukaReakcije = 1f;

        [Header("Animacija nakon reakcije")]
        [Tooltip(
            "Animacija koja se pokreće nakon prve reakcije. " +
            "Primjer: Angry ili GetUp. Ostavi prazno ako nije potrebna."
        )]
        public string triggerNakonReakcije = "";

        [Tooltip(
            "Točan naziv Animator stanja nakon reakcije. " +
            "Primjer: Angry ili GetUp."
        )]
        public string stanjeNakonReakcije = "";

        [Tooltip(
            "Najduže čekanje završetka animacije nakon reakcije."
        )]
        public float maksimalnoTrajanjeNakonReakcije = 10f;

        [Header("Smjer gledanja tijekom animacije nakon reakcije")]
        [Tooltip(
            "Smjer u kojem NPC gleda tijekom animacije nakon reakcije. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaTijekomNakonReakcije =
            SmjerGledanja.Automatski;

        [Header("Smjer gledanja nakon animacije nakon reakcije")]
        [Tooltip(
            "Smjer u kojem NPC gleda nakon što animacija nakon reakcije završi. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaNakonAnimacijeNakonReakcije =
            SmjerGledanja.Automatski;

        [Header("Zvuk animacije nakon reakcije")]
        [Tooltip("Zvuk dodatne reakcije, primjerice Angry ili GetUp.")]
        public AudioClip zvukNakonReakcije;

        [Range(0f, 1f)]
        public float glasnocaZvukaNakonReakcije = 1f;

        [Header("Animacija čišćenja zamke")]
        [Tooltip(
            "Animacija kojom NPC pokupi ili očisti zamku. " +
            "Primjer: Pickup. Ostavi prazno ako nije potrebna."
        )]
        public string triggerCiscenjaZamke = "";

        [Tooltip(
            "Točan naziv Animator stanja čišćenja zamke. " +
            "Primjer: Pickup."
        )]
        public string stanjeCiscenjaZamke = "";

        [Tooltip(
            "Najduže čekanje završetka animacije čišćenja."
        )]
        public float maksimalnoTrajanjeCiscenjaZamke = 10f;

        [Header("Smjer gledanja tijekom animacije čišćenja zamke")]
        [Tooltip(
            "Smjer u kojem NPC gleda tijekom animacije čišćenja zamke. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaTijekomCiscenjaZamke =
            SmjerGledanja.Automatski;

        [Header("Smjer gledanja nakon animacije čišćenja zamke")]
        [Tooltip(
            "Smjer u kojem NPC gleda nakon što animacija čišćenja zamke završi. " +
            "Automatski ostavlja trenutni smjer."
        )]
        public SmjerGledanja smjerGledanjaNakonCiscenjaZamke =
            SmjerGledanja.Automatski;

        [Header("Zvuk animacije čišćenja zamke")]
        [Tooltip("Zvuk koji se reproducira kada počne animacija čišćenja zamke.")]
        public AudioClip zvukCiscenjaZamke;

        [Range(0f, 1f)]
        public float glasnocaZvukaCiscenjaZamke = 1f;

        [Header("Čekanje")]
        [Tooltip(
            "Koliko NPC miruje prije pokretanja animacije radnje."
        )]
        public float trajanjeRadnje = 2f;

        [Tooltip("Pauza nakon cijele radnje i reakcije.")]
        public float pauzaNakonRadnje = 1f;

        [HideInInspector]
        public bool zamkaIskoristena = false;
    }

    [Header("Rutina NPC-a")]
    public KorakRutine[] rutina;

    [Header("Kretanje")]
    public float brzinaHodanja = 2f;
    public float udaljenostZaDolazak = 0.4f;
    public float maksimalnoVrijemePutovanja = 30f;

    [Header("Glavni Animator")]
    public Animator animator;

    [Header("Svi NPC Animatori")]
    [Tooltip("Dodaj Animator tijela i Animator outfita.")]
    public Animator[] npcAnimatori;

    [Header("Animator parametri")]
    public string walkingBool = "isWalking";

    [Header("Audio Source")]
    [Tooltip("AudioSource preko kojeg NPC reproducira zvukove animacija.")]
    public AudioSource audioSource;

    private NavMeshAgent agent;
    private bool rutinaPokrenuta = false;

    private const float ROTACIJA_DESNO = 84.79f;
    private const float ROTACIJA_LIJEVO = 264.79f;
    private const float ROTACIJA_NAPRIJED = 354.79f;
    private const float ROTACIJA_NAZAD = 174.79f;

    private Transform modelNPC;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError(
                "NPC nema NavMeshAgent komponentu."
            );

            enabled = false;
            return;
        }

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>(true);
        }

        if (animator != null)
        {
            modelNPC = animator.transform;
        }
        else
        {
            Debug.LogWarning(
                "Glavni Animator nije postavljen. " +
                "Ručno gledanje i animacije neće raditi."
            );
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        agent.speed = brzinaHodanja;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.updateUpAxis = true;

        PostaviHodanje(false);

        Debug.Log(
            "Ukupno mogućih bodova iz svih zamki: " +
            DohvatiUkupneBodoveZamki()
        );

        if (rutina != null &&
            rutina.Length > 0)
        {
            StartCoroutine(
                PokreniRutinu()
            );
        }
        else
        {
            Debug.LogWarning(
                "NPC nema postavljenu rutinu."
            );
        }
    }

    private IEnumerator PokreniRutinu()
    {
        if (rutinaPokrenuta)
        {
            yield break;
        }

        rutinaPokrenuta = true;

        while (true)
        {
            for (int i = 0; i < rutina.Length; i++)
            {
                KorakRutine korak =
                    rutina[i];

                if (korak == null)
                {
                    continue;
                }

                if (korak.mjestoDolaska == null)
                {
                    Debug.LogWarning(
                        "Nedostaje mjesto dolaska za korak: " +
                        korak.nazivKoraka
                    );

                    continue;
                }

                yield return StartCoroutine(
                    IdiDoMjesta(
                        korak.mjestoDolaska
                    )
                );

                PostaviSmjerGledanja(
                    korak.smjerGledanjaNakonDolaska
                );

                if (korak.teleportiraj)
                {
                    TeleportirajNPC(
                        korak.teleportIzlaz
                    );

                    PustiZvuk(
                        korak.zvukOtvaranjaVrata,
                        korak.glasnocaZvukaOtvaranjaVrata
                    );

                    yield return new WaitForSeconds(
                        0.2f
                    );

                    PostaviSmjerGledanja(
                        korak.smjerGledanjaNakonRadnje
                    );

                    continue;
                }

                yield return StartCoroutine(
                    IzvrsiRadnju(korak)
                );

                PostaviSmjerGledanja(
                    korak.smjerGledanjaNakonRadnje
                );

                yield return new WaitForSeconds(
                    Mathf.Max(
                        0f,
                        korak.pauzaNakonRadnje
                    )
                );
            }
        }
    }

    private IEnumerator IdiDoMjesta(
        Transform cilj)
    {
        if (cilj == null)
        {
            yield break;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "NPC nije na NavMesh površini."
            );

            yield break;
        }

        bool pronadenaPozicija =
            NavMesh.SamplePosition(
                cilj.position,
                out NavMeshHit pogodak,
                2f,
                NavMesh.AllAreas
            );

        if (!pronadenaPozicija)
        {
            Debug.LogWarning(
                "Mjesto nije na NavMeshu: " +
                cilj.name
            );

            yield break;
        }

        agent.isStopped = false;

        bool putanjaPostavljena =
            agent.SetDestination(
                pogodak.position
            );

        if (!putanjaPostavljena)
        {
            Debug.LogWarning(
                "NPC ne može krenuti prema: " +
                cilj.name
            );

            PostaviHodanje(false);
            yield break;
        }

        PostaviHodanje(true);

        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer >=
                maksimalnoVrijemePutovanja)
            {
                Debug.LogWarning(
                    "NPC je predugo pokušavao doći do: " +
                    cilj.name
                );

                break;
            }

            if (!agent.pathPending)
            {
                if (agent.pathStatus ==
                    NavMeshPathStatus.PathInvalid)
                {
                    Debug.LogWarning(
                        "Putanja nije ispravna do: " +
                        cilj.name
                    );

                    break;
                }

                if (agent.remainingDistance <=
                    agent.stoppingDistance +
                    udaljenostZaDolazak)
                {
                    break;
                }
            }

            yield return null;
        }

        ZaustaviNPC();
    }

    private void PostaviSmjerGledanja(
        SmjerGledanja smjer)
    {
        if (smjer ==
            SmjerGledanja.Automatski)
        {
            return;
        }

        if (modelNPC == null)
        {
            return;
        }

        Vector3 trenutnaRotacija =
            modelNPC.localEulerAngles;

        float novaRotacijaY =
            ROTACIJA_DESNO;

        switch (smjer)
        {
            case SmjerGledanja.Lijevo:
                novaRotacijaY =
                    ROTACIJA_LIJEVO;
                break;

            case SmjerGledanja.Desno:
                novaRotacijaY =
                    ROTACIJA_DESNO;
                break;

            case SmjerGledanja.Naprijed:
                novaRotacijaY =
                    ROTACIJA_NAPRIJED;
                break;

            case SmjerGledanja.Nazad:
                novaRotacijaY =
                    ROTACIJA_NAZAD;
                break;
        }

        modelNPC.localRotation =
            Quaternion.Euler(
                trenutnaRotacija.x,
                novaRotacijaY,
                trenutnaRotacija.z
            );
    }

    private IEnumerator IzvrsiRadnju(
        KorakRutine korak)
    {
        ZaustaviNPC();

        PostaviSmjerGledanja(
            korak.smjerGledanjaTijekomRadnje
        );

        yield return new WaitForSeconds(
            Mathf.Max(
                0f,
                korak.trajanjeRadnje
            )
        );


        yield return StartCoroutine(
            PokreniICekajAnimacijuRadnje(korak)
        );


        bool zamkaJeAktivna =
            korak.aktivnaZamka != null &&
            korak.aktivnaZamka.activeInHierarchy;


        if (!zamkaJeAktivna)
        {
            yield return StartCoroutine(
                PokreniICekajAnimaciju(
                    korak.triggerDokNemaZamke,
                    korak.stanjeDokNemaZamke,
                    korak.maksimalnoTrajanjeDokNemaZamke,
                    "dok nema zamke",
                    korak.nazivKoraka,
                    korak.zvukDokNemaZamke,
                    korak.glasnocaZvukaDokNemaZamke,
                    korak.smjerGledanjaDokNemaZamke,
                    korak.smjerGledanjaNakonDokNemaZamke
                )
            );

            Debug.Log(
                "NPC je završio korak bez aktivne zamke: " +
                korak.nazivKoraka
            );

            yield break;
        }


        if (korak.zamkaIskoristena)
        {
            Debug.Log(
                "Zamka je već iskorištena: " +
                korak.nazivKoraka
            );

            yield break;
        }

        korak.zamkaIskoristena = true;

        Debug.Log(
            "NPC je pronašao zamku: " +
            korak.nazivKoraka
        );

        if (korak.efektZamke != null)
        {
            korak.efektZamke.Play();
        }

        ZaustaviNPC();

        PostaviSmjerGledanja(
            korak.smjerGledanjaTijekomReakcije
        );

        PokreniTriggerNaSvimAnimatorima(
            korak.triggerReakcije
        );

        PustiZvuk(
            korak.zvukReakcije,
            korak.glasnocaZvukaReakcije
        );

        if (ScoreManager.instance != null)
        {
            int osvojeniBodovi =
                Mathf.Max(
                    0,
                    korak.bodoviZaZamku
                );

            ScoreManager.instance.DodajBodove(
                osvojeniBodovi
            );

            Debug.Log(
                "Dodano bodova: " +
                osvojeniBodovi
            );
        }
        else
        {
            Debug.LogWarning(
                "ScoreManager nije pronađen u sceni."
            );
        }


        yield return StartCoroutine(
            CekajZavrsetakAnimacije(
                korak.stanjeReakcije,
                korak.maksimalnoTrajanjeReakcije,
                "reakcije"
            )
        );

        PostaviSmjerGledanja(
            korak.smjerGledanjaNakonReakcije
        );

        ZaustaviNPC();


        yield return StartCoroutine(
            PokreniICekajAnimaciju(
                korak.triggerNakonReakcije,
                korak.stanjeNakonReakcije,
                korak.maksimalnoTrajanjeNakonReakcije,
                "nakon reakcije",
                korak.nazivKoraka,
                korak.zvukNakonReakcije,
                korak.glasnocaZvukaNakonReakcije,
                korak.smjerGledanjaTijekomNakonReakcije,
                korak.smjerGledanjaNakonAnimacijeNakonReakcije
            )
        );

        ZaustaviNPC();


        yield return StartCoroutine(
            PokreniICekajAnimaciju(
                korak.triggerCiscenjaZamke,
                korak.stanjeCiscenjaZamke,
                korak.maksimalnoTrajanjeCiscenjaZamke,
                "čišćenja zamke",
                korak.nazivKoraka,
                korak.zvukCiscenjaZamke,
                korak.glasnocaZvukaCiscenjaZamke,
                korak.smjerGledanjaTijekomCiscenjaZamke,
                korak.smjerGledanjaNakonCiscenjaZamke
            )
        );

        ZaustaviNPC();


        if (korak.aktivnaZamka != null)
        {
            korak.aktivnaZamka.SetActive(
                false
            );

            Debug.Log(
                "NPC je uklonio zamku: " +
                korak.nazivKoraka
            );
        }
    }

    private IEnumerator PokreniICekajAnimacijuRadnje(
        KorakRutine korak)
    {
        if (korak == null)
        {
            yield break;
        }

        if (string.IsNullOrEmpty(
            korak.triggerRadnje))
        {
            yield break;
        }

        if (string.IsNullOrEmpty(
            korak.stanjeRadnje))
        {
            Debug.LogWarning(
                "Trigger radnje je postavljen, ali naziv " +
                "Animator stanja radnje nije postavljen za: " +
                korak.nazivKoraka
            );

            yield break;
        }

        ZaustaviNPC();

        PostaviSmjerGledanja(
            korak.smjerGledanjaTijekomRadnje
        );

        PokreniTriggerNaSvimAnimatorima(
            korak.triggerRadnje
        );

        PustiZvuk(
            korak.zvukRadnje,
            korak.glasnocaZvukaRadnje
        );

        Debug.Log(
            "Pokrenuta animacija radnje: " +
            korak.stanjeRadnje +
            " za korak: " +
            korak.nazivKoraka
        );

        yield return StartCoroutine(
            CekajZavrsetakAnimacije(
                korak.stanjeRadnje,
                korak.maksimalnoTrajanjeAnimacijeRadnje,
                "radnje"
            )
        );

        ZaustaviNPC();

        Debug.Log(
            "Završena animacija radnje: " +
            korak.stanjeRadnje +
            " za korak: " +
            korak.nazivKoraka
        );
    }

    private IEnumerator PokreniICekajAnimaciju(
        string nazivTriggera,
        string nazivStanja,
        float maksimalnoVrijeme,
        string vrstaAnimacije,
        string nazivKoraka,
        AudioClip zvuk = null,
        float glasnocaZvuka = 1f,
        SmjerGledanja smjerGledanja = SmjerGledanja.Automatski,
        SmjerGledanja smjerGledanjaNakon = SmjerGledanja.Automatski)
    {
        if (string.IsNullOrEmpty(
            nazivTriggera))
        {
            yield break;
        }

        if (string.IsNullOrEmpty(
            nazivStanja))
        {
            Debug.LogWarning(
                "Trigger animacije " +
                vrstaAnimacije +
                " je postavljen, ali naziv stanja nije postavljen za: " +
                nazivKoraka
            );

            yield break;
        }

        ZaustaviNPC();

        PostaviSmjerGledanja(
            smjerGledanja
        );

        PokreniTriggerNaSvimAnimatorima(
            nazivTriggera
        );

        PustiZvuk(
            zvuk,
            glasnocaZvuka
        );

        Debug.Log(
            "Pokrenuta animacija " +
            vrstaAnimacije +
            ": " +
            nazivStanja +
            " za korak: " +
            nazivKoraka
        );

        yield return StartCoroutine(
            CekajZavrsetakAnimacije(
                nazivStanja,
                maksimalnoVrijeme,
                vrstaAnimacije
            )
        );

        PostaviSmjerGledanja(
            smjerGledanjaNakon
        );

        ZaustaviNPC();

        Debug.Log(
            "Završena animacija " +
            vrstaAnimacije +
            ": " +
            nazivStanja +
            " za korak: " +
            nazivKoraka
        );
    }

    private IEnumerator CekajZavrsetakAnimacije(
        string nazivStanja,
        float maksimalnoVrijeme,
        string vrstaAnimacije)
    {
        if (animator == null)
        {
            Debug.LogWarning(
                "Glavni Animator nije postavljen. " +
                "Animacija " +
                vrstaAnimacije +
                " ne može se pratiti."
            );

            yield return new WaitForSeconds(
                Mathf.Max(
                    0.1f,
                    maksimalnoVrijeme
                )
            );

            yield break;
        }

        if (string.IsNullOrEmpty(
            nazivStanja))
        {
            Debug.LogWarning(
                "Naziv stanja animacije " +
                vrstaAnimacije +
                " nije postavljen."
            );

            yield break;
        }

        float sigurnoMaksimalnoVrijeme =
            Mathf.Max(
                0.5f,
                maksimalnoVrijeme
            );

        float timer = 0f;
        bool animacijaJePocela = false;


        while (timer <
               sigurnoMaksimalnoVrijeme)
        {
            ZaustaviNPC();

            AnimatorStateInfo trenutnoStanje =
                animator.GetCurrentAnimatorStateInfo(
                    0
                );

            AnimatorStateInfo sljedeceStanje =
                animator.GetNextAnimatorStateInfo(
                    0
                );

            bool trenutnoJeAnimacija =
                trenutnoStanje.IsName(
                    nazivStanja
                );

            bool sljedeceJeAnimacija =
                animator.IsInTransition(0) &&
                sljedeceStanje.IsName(
                    nazivStanja
                );

            if (trenutnoJeAnimacija ||
                sljedeceJeAnimacija)
            {
                animacijaJePocela = true;
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        if (!animacijaJePocela)
        {
            Debug.LogWarning(
                "Animator nije ušao u stanje " +
                vrstaAnimacije +
                ": " +
                nazivStanja +
                ". Provjeri Trigger i naziv stanja."
            );

            yield break;
        }

        timer = 0f;

 
        while (timer <
               sigurnoMaksimalnoVrijeme)
        {
            ZaustaviNPC();

            AnimatorStateInfo trenutnoStanje =
                animator.GetCurrentAnimatorStateInfo(
                    0
                );

            bool animatorJeUPrijelazu =
                animator.IsInTransition(0);

            bool trenutnoJeAnimacija =
                trenutnoStanje.IsName(
                    nazivStanja
                );

            bool animacijaJeDoslaDoKraja =
                trenutnoJeAnimacija &&
                trenutnoStanje.normalizedTime >= 1f;

            if (animacijaJeDoslaDoKraja &&
                animatorJeUPrijelazu)
            {
                break;
            }

            if (!trenutnoJeAnimacija &&
                !animatorJeUPrijelazu)
            {
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        if (timer >=
            sigurnoMaksimalnoVrijeme)
        {
            Debug.LogWarning(
                "Predugo se čekao završetak animacije " +
                vrstaAnimacije +
                ": " +
                nazivStanja
            );
        }

        timer = 0f;

       
        while (animator.IsInTransition(0) &&
               timer < 2f)
        {
            ZaustaviNPC();

            timer += Time.deltaTime;

            yield return null;
        }

        Debug.Log(
            "Animacija " +
            vrstaAnimacije +
            " je završena: " +
            nazivStanja
        );
    }

    private void PustiZvuk(
        AudioClip zvuk,
        float glasnoca)
    {
        if (audioSource == null ||
            zvuk == null)
        {
            return;
        }

        audioSource.PlayOneShot(
            zvuk,
            Mathf.Clamp01(glasnoca)
        );
    }

    public int DohvatiUkupneBodoveZamki()
    {
        int ukupno = 0;

        if (rutina == null)
        {
            return ukupno;
        }

        foreach (KorakRutine korak in rutina)
        {
            if (korak == null)
            {
                continue;
            }

            if (korak.aktivnaZamka == null)
            {
                continue;
            }

            ukupno += Mathf.Max(
                0,
                korak.bodoviZaZamku
            );
        }

        return ukupno;
    }

    private void TeleportirajNPC(
        Transform izlaz)
    {
        if (izlaz == null)
        {
            Debug.LogWarning(
                "Teleport izlaz nije postavljen."
            );

            return;
        }

        bool pronadenaPozicija =
            NavMesh.SamplePosition(
                izlaz.position,
                out NavMeshHit pogodak,
                2f,
                NavMesh.AllAreas
            );

        if (!pronadenaPozicija)
        {
            Debug.LogWarning(
                "Teleport izlaz nije na NavMeshu: " +
                izlaz.name
            );

            return;
        }

        ZaustaviNPC();

        bool teleportUspio =
            agent.Warp(
                pogodak.position
            );

        if (!teleportUspio)
        {
            Debug.LogWarning(
                "Teleport nije uspio prema: " +
                izlaz.name
            );

            return;
        }

        transform.rotation =
            izlaz.rotation;

        PostaviHodanje(false);

        Debug.Log(
            "NPC je promijenio kat."
        );
    }

    private void ZaustaviNPC()
    {
        if (agent != null &&
            agent.enabled &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        PostaviHodanje(false);
    }

    private void PostaviHodanje(
        bool hoda)
    {
        PostaviBoolNaAnimatoru(
            animator,
            walkingBool,
            hoda
        );

        if (npcAnimatori == null)
        {
            return;
        }

        foreach (
            Animator npcAnimator
            in npcAnimatori)
        {
            if (npcAnimator == null ||
                npcAnimator == animator)
            {
                continue;
            }

            PostaviBoolNaAnimatoru(
                npcAnimator,
                walkingBool,
                hoda
            );
        }
    }

    private void PostaviBoolNaAnimatoru(
        Animator ciljaniAnimator,
        string nazivParametra,
        bool vrijednost)
    {
        if (ciljaniAnimator == null ||
            string.IsNullOrEmpty(
                nazivParametra
            ))
        {
            return;
        }

        if (!AnimatorImaParametar(
                ciljaniAnimator,
                nazivParametra,
                AnimatorControllerParameterType.Bool))
        {
            return;
        }

        ciljaniAnimator.SetBool(
            nazivParametra,
            vrijednost
        );
    }

    private void PokreniTriggerNaSvimAnimatorima(
        string nazivTriggera)
    {
        PokreniTriggerNaAnimatoru(
            animator,
            nazivTriggera
        );

        if (npcAnimatori == null)
        {
            return;
        }

        foreach (
            Animator npcAnimator
            in npcAnimatori)
        {
            if (npcAnimator == null ||
                npcAnimator == animator)
            {
                continue;
            }

            PokreniTriggerNaAnimatoru(
                npcAnimator,
                nazivTriggera
            );
        }
    }

    private void PokreniTriggerNaAnimatoru(
        Animator ciljaniAnimator,
        string nazivTriggera)
    {
        if (ciljaniAnimator == null ||
            string.IsNullOrEmpty(
                nazivTriggera
            ))
        {
            return;
        }

        if (!AnimatorImaParametar(
            ciljaniAnimator,
            nazivTriggera,
            AnimatorControllerParameterType.Trigger))
        {
            Debug.LogWarning(
                "Animator " +
                ciljaniAnimator.name +
                " nema Trigger parametar: " +
                nazivTriggera
            );

            return;
        }

        ciljaniAnimator.ResetTrigger(
            nazivTriggera
        );

        ciljaniAnimator.SetTrigger(
            nazivTriggera
        );
    }

    private bool AnimatorImaParametar(
        Animator ciljaniAnimator,
        string nazivParametra,
        AnimatorControllerParameterType tipParametra)
    {
        if (ciljaniAnimator == null)
        {
            return false;
        }

        foreach (
            AnimatorControllerParameter parametar
            in ciljaniAnimator.parameters)
        {
            if (parametar.name ==
                nazivParametra &&
                parametar.type ==
                tipParametra)
            {
                return true;
            }
        }

        return false;
    }
}
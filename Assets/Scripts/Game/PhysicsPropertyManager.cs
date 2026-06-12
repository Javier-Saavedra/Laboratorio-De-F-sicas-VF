using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PhysicsPropertyManager : MonoBehaviour
{
    // ─── ORIGINAL — no se tocó nada ───────────────────────────────────────────

    [Tooltip("The keyboard button to toggle the time scale.")]
    public KeyCode timeScaleKey = KeyCode.Z;
    [Tooltip("The altered time scale.")]
    public float timeScale;
    [Tooltip("The UI text to show the time key button.")]
    public TextMeshProUGUI timeKeyText;
    [Tooltip("The UI image for the time mode.")]
    public Image timeImage;

    [Space]
    [Tooltip("The keyboard button to toggle the gravity.")]
    public KeyCode gravityKey = KeyCode.X;
    [Tooltip("The alterated gravity vector.")]
    public Vector3 fakeGravity;
    [Tooltip("The UI text to show the gravity key button.")]
    public TextMeshProUGUI gravityKeyText;
    [Tooltip("The UI image for the gravity changed mode.")]
    public Image gravityOnImage;
    [Tooltip("The UI image for the gravity default mode.")]
    public Image gravityOffImage;
    [Tooltip("Post Process manager")]
    public PostProcessManager postProcessManager;
    [Tooltip("The Audio Controller")]
    public AudioController audioController;

    bool timeToggle;
    bool gravityToggle;
    float defaultTimeScale;
    float defaultFixedDeltaTime;
    Vector3 defaultGravity;

    // ─── NUEVO — Fuerzas generadoras ──────────────────────────────────────────

    [Header("── VIENTO ──────────────────────────────")]
    [Tooltip("Tecla para activar/desactivar el viento.")]
    public KeyCode vientoKey = KeyCode.H;
    public TextMeshProUGUI vientoKeyText;
    public Image vientoOnImage;
    public Image vientoOffImage;

    [Header("── EXPLOSIÓN ───────────────────────────")]
    [Tooltip("Tecla para detonar la explosión.")]
    public KeyCode explosionKey = KeyCode.J;
    public TextMeshProUGUI explosionKeyText;
    public Image explosionImage;

    [Header("── MAGNETISMO ──────────────────────────")]
    [Tooltip("Tecla para activar/desactivar el magnetismo.")]
    public KeyCode magnetismoKey = KeyCode.K;
    public TextMeshProUGUI magnetismoKeyText;
    public Image magnetismoOnImage;
    public Image magnetismoOffImage;

    [Header("── VÓRTICE ─────────────────────────────")]
    [Tooltip("Tecla para activar/desactivar el vórtice.")]
    public KeyCode vorticeKey = KeyCode.L;
    public TextMeshProUGUI vorticeKeyText;
    public Image vorticeOnImage;
    public Image vorticeOffImage;

    [Header("── REPULSIÓN ───────────────────────────")]
    [Tooltip("Tecla para activar/desactivar la repulsión.")]
    public KeyCode repulsionKey = KeyCode.N;
    public TextMeshProUGUI repulsionKeyText;
    public Image repulsionOnImage;
    public Image repulsionOffImage;

    // Parámetros de cada fuerza — ajustables desde el Inspector
    [Header("Parámetros — Viento")]
    public Vector3 windDirection = Vector3.right;
    public float windStrength = 10f;
    public float windTurbulence = 2f;

    [Header("Parámetros — Explosión")]
    public float explosionStrength = 800f;
    public float explosionRadius = 8f;

    [Header("Parámetros — Magnetismo")]
    public float magnetismoG = 20f;
    public float magnetismoRadius = 8f;

    [Header("Parámetros — Vórtice")]
    public float vorticeTangential = 15f;
    public float vorticeRadius = 7f;
    public float vorticeLift = 3f;

    [Header("Parámetros — Repulsión")]
    public float repulsionStrength = 30f;
    public float repulsionRadius = 6f;

    // ─── NUEVO — Panel de Propiedades ─────────────────────────────────────────

    [Header("── PANEL DE PROPIEDADES ────────────────")]
    public GameObject subPanelViento;
    public GameObject subPanelExplosion;
    public GameObject subPanelMagnetismo;
    public GameObject subPanelVortice;
    public GameObject subPanelRepulsion;

    [Header("Sliders — Viento")]
    public Slider sliderVientoFuerza;
    public Slider sliderVientoTurbulencia;
    public TextMeshProUGUI textoVientoFuerza;
    public TextMeshProUGUI textoVientoTurbulencia;

    [Header("Sliders — Explosión")]
    public Slider sliderExplosionFuerza;
    public Slider sliderExplosionRadio;
    public TextMeshProUGUI textoExplosionFuerza;
    public TextMeshProUGUI textoExplosionRadio;

    [Header("Sliders — Magnetismo")]
    public Slider sliderMagnetismoG;
    public Slider sliderMagnetismoRadio;
    public TextMeshProUGUI textoMagnetismoG;
    public TextMeshProUGUI textoMagnetismoRadio;

    [Header("Sliders — Vórtice")]
    public Slider sliderVorticeRotacion;
    public Slider sliderVorticeRadio;
    public TextMeshProUGUI textoVorticeRotacion;
    public TextMeshProUGUI textoVorticeRadio;

    [Header("Sliders — Repulsión")]
    public Slider sliderRepulsionFuerza;
    public Slider sliderRepulsionRadio;
    public TextMeshProUGUI textoRepulsionFuerza;
    public TextMeshProUGUI textoRepulsionRadio;

    // Toggles internos de fuerzas
    bool vientoToggle;
    bool magnetismoToggle;
    bool vorticeToggle;
    bool repulsionToggle;

    // Constantes Perlin para viento
    const float OFFSET_X = 0f;
    const float OFFSET_Y = 31.41f;
    const float OFFSET_Z = 72.83f;

    // ─── START ────────────────────────────────────────────────────────────────

    void Start()
    {
        // Original
        defaultTimeScale      = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        defaultGravity        = Physics.gravity;
        timeKeyText.text    = timeScaleKey.ToString().ToUpper();
        gravityKeyText.text = gravityKey.ToString().ToUpper();
        setTimeImage(timeToggle);
        setGravityImages(gravityToggle);

        // Textos de teclas nuevas
        if (vientoKeyText)     vientoKeyText.text     = vientoKey.ToString().ToUpper();
        if (explosionKeyText)  explosionKeyText.text   = explosionKey.ToString().ToUpper();
        if (magnetismoKeyText) magnetismoKeyText.text  = magnetismoKey.ToString().ToUpper();
        if (vorticeKeyText)    vorticeKeyText.text     = vorticeKey.ToString().ToUpper();
        if (repulsionKeyText)  repulsionKeyText.text   = repulsionKey.ToString().ToUpper();

        // Iconos iniciales apagados
        SetImages(vientoOnImage,     vientoOffImage,     false);
        SetImages(magnetismoOnImage, magnetismoOffImage, false);
        SetImages(vorticeOnImage,    vorticeOffImage,    false);
        SetImages(repulsionOnImage,  repulsionOffImage,  false);
        SetSingleImage(explosionImage, false);

        // Panel de propiedades — todos ocultos al inicio
        MostrarPanel(null);
        ConfigurarSliders();
    }

    // ─── PANEL DE PROPIEDADES ─────────────────────────────────────────────────

    /// <summary>
    /// Muestra solo el sub-panel indicado y oculta los demás.
    /// Si se pasa null, oculta todos.
    /// </summary>
    void MostrarPanel(GameObject panelAMostrar)
    {
        if (subPanelViento)     subPanelViento.SetActive(panelAMostrar == subPanelViento);
        if (subPanelExplosion)  subPanelExplosion.SetActive(panelAMostrar == subPanelExplosion);
        if (subPanelMagnetismo) subPanelMagnetismo.SetActive(panelAMostrar == subPanelMagnetismo);
        if (subPanelVortice)    subPanelVortice.SetActive(panelAMostrar == subPanelVortice);
        if (subPanelRepulsion)  subPanelRepulsion.SetActive(panelAMostrar == subPanelRepulsion);
    }

    void ConfigurarSliders()
    {
        ConfigSlider(sliderVientoFuerza, 0, 30, windStrength, v => {
            windStrength = v;
            if (textoVientoFuerza) textoVientoFuerza.text = $"Fuerza: {v:F1}";
        });
        ConfigSlider(sliderVientoTurbulencia, 0, 10, windTurbulence, v => {
            windTurbulence = v;
            if (textoVientoTurbulencia) textoVientoTurbulencia.text = $"Turbulencia: {v:F1}";
        });

        ConfigSlider(sliderExplosionFuerza, 100, 2000, explosionStrength, v => {
            explosionStrength = v;
            if (textoExplosionFuerza) textoExplosionFuerza.text = $"Fuerza: {v:F0}";
        });
        ConfigSlider(sliderExplosionRadio, 1, 20, explosionRadius, v => {
            explosionRadius = v;
            if (textoExplosionRadio) textoExplosionRadio.text = $"Radio: {v:F1}";
        });

        ConfigSlider(sliderMagnetismoG, -50, 50, magnetismoG, v => {
            magnetismoG = v;
            if (textoMagnetismoG) textoMagnetismoG.text = $"G: {v:F1}";
        });
        ConfigSlider(sliderMagnetismoRadio, 1, 20, magnetismoRadius, v => {
            magnetismoRadius = v;
            if (textoMagnetismoRadio) textoMagnetismoRadio.text = $"Radio: {v:F1}";
        });

        ConfigSlider(sliderVorticeRotacion, 0, 40, vorticeTangential, v => {
            vorticeTangential = v;
            if (textoVorticeRotacion) textoVorticeRotacion.text = $"Rotación: {v:F1}";
        });
        ConfigSlider(sliderVorticeRadio, 1, 20, vorticeRadius, v => {
            vorticeRadius = v;
            if (textoVorticeRadio) textoVorticeRadio.text = $"Radio: {v:F1}";
        });

        ConfigSlider(sliderRepulsionFuerza, 0, 100, repulsionStrength, v => {
            repulsionStrength = v;
            if (textoRepulsionFuerza) textoRepulsionFuerza.text = $"Fuerza: {v:F1}";
        });
        ConfigSlider(sliderRepulsionRadio, 1, 20, repulsionRadius, v => {
            repulsionRadius = v;
            if (textoRepulsionRadio) textoRepulsionRadio.text = $"Radio: {v:F1}";
        });
    }

    void ConfigSlider(Slider s, float min, float max, float valor, UnityEngine.Events.UnityAction<float> onChange)
    {
        if (s == null) return;
        s.minValue = min;
        s.maxValue = max;
        s.value = valor;
        s.onValueChanged.AddListener(onChange);
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────────

    void Update()
    {
        // ── Original ──
        if (Input.GetKeyUp(timeScaleKey))
        {
            timeToggle = !timeToggle;
            Time.timeScale = timeToggle ? timeScale : defaultTimeScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            setTimeImage(timeToggle);
            if (postProcessManager != null)
                postProcessManager.ChangeVolume(timeToggle ? Mode.timescale : Mode.defaultTimeScale);
            if (audioController != null)
            {
                if (timeToggle) audioController.HalfPitchAudio();
                else audioController.FullSpeedAudio();
            }
        }

        if (Input.GetKeyUp(gravityKey))
        {
            gravityToggle = !gravityToggle;
            Physics.gravity = gravityToggle ? fakeGravity : defaultGravity;
            setGravityImages(gravityToggle);
            if (postProcessManager != null)
                postProcessManager.ChangeVolume(gravityToggle ? Mode.customGravity : Mode.standardGravity);
            if (audioController != null)
            {
                if (gravityToggle) audioController.AudioReverse();
                else audioController.AudioForward();
            }
        }

        // ── Nuevas fuerzas ──
        if (Input.GetKeyUp(vientoKey))
        {
            vientoToggle = !vientoToggle;
            SetImages(vientoOnImage, vientoOffImage, vientoToggle);
            MostrarPanel(vientoToggle ? subPanelViento : null);
        }

        if (Input.GetKeyUp(explosionKey))
        {
            Detonate();
            SetSingleImage(explosionImage, true);
            Invoke(nameof(ResetExplosionImage), 0.3f);
            MostrarPanel(subPanelExplosion);
        }

        if (Input.GetKeyUp(magnetismoKey))
        {
            magnetismoToggle = !magnetismoToggle;
            SetImages(magnetismoOnImage, magnetismoOffImage, magnetismoToggle);
            MostrarPanel(magnetismoToggle ? subPanelMagnetismo : null);
        }

        if (Input.GetKeyUp(vorticeKey))
        {
            vorticeToggle = !vorticeToggle;
            SetImages(vorticeOnImage, vorticeOffImage, vorticeToggle);
            MostrarPanel(vorticeToggle ? subPanelVortice : null);
        }

        if (Input.GetKeyUp(repulsionKey))
        {
            repulsionToggle = !repulsionToggle;
            SetImages(repulsionOnImage, repulsionOffImage, repulsionToggle);
            MostrarPanel(repulsionToggle ? subPanelRepulsion : null);
        }
    }

    // ─── FIXED UPDATE — aplica fuerzas ───────────────────────────────────────

    void FixedUpdate()
    {
        if (!vientoToggle && !magnetismoToggle && !vorticeToggle && !repulsionToggle)
            return;

        Rigidbody[] bodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        foreach (Rigidbody rb in bodies)
        {
            if (rb.isKinematic) continue;

            // Viento
            if (vientoToggle)
            {
                Vector3 baseForce = windDirection.normalized * windStrength;
                float t = Time.time * 0.8f;
                Vector3 turb = new Vector3(
                    (Mathf.PerlinNoise(t + OFFSET_X, 0f) * 2f - 1f) * windTurbulence,
                    (Mathf.PerlinNoise(t + OFFSET_Y, 0f) * 2f - 1f) * windTurbulence,
                    (Mathf.PerlinNoise(t + OFFSET_Z, 0f) * 2f - 1f) * windTurbulence
                );
                rb.AddForce(baseForce + turb, ForceMode.Force);
            }

            // Magnetismo
            if (magnetismoToggle)
            {
                Vector3 toCenter = transform.position - rb.position;
                float dist = Mathf.Max(toCenter.magnitude, 0.3f);
                if (dist <= magnetismoRadius)
                {
                    float mag = magnetismoG * rb.mass / (dist * dist);
                    rb.AddForce(toCenter.normalized * mag, ForceMode.Force);
                }
            }

            // Vórtice
            if (vorticeToggle)
            {
                Vector3 toBody = rb.position - transform.position;
                Vector3 axisComp = Vector3.Dot(toBody, Vector3.up) * Vector3.up;
                Vector3 radial = toBody - axisComp;
                float dist = Mathf.Max(radial.magnitude, 0.2f);
                if (dist <= vorticeRadius)
                {
                    Vector3 tangent = Vector3.Cross(Vector3.up, radial).normalized;
                    rb.AddForce(
                        tangent * vorticeTangential / dist +
                        -radial.normalized * 5f / dist +
                        Vector3.up * vorticeLift,
                        ForceMode.Force
                    );
                }
            }

            // Repulsión
            if (repulsionToggle)
            {
                Vector3 toBody = rb.position - transform.position;
                float dist = Mathf.Max(toBody.magnitude, 0.5f);
                if (dist <= repulsionRadius)
                    rb.AddForce(toBody.normalized * repulsionStrength / (dist * dist), ForceMode.Force);
            }
        }
    }

    // ─── EXPLOSIÓN ────────────────────────────────────────────────────────────

    void Detonate()
    {
        Rigidbody[] bodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach (Rigidbody rb in bodies)
        {
            if (rb.isKinematic) continue;
            Vector3 toBody = rb.position - transform.position;
            float dist = toBody.magnitude;
            if (dist <= Mathf.Epsilon || dist > explosionRadius) continue;
            float falloff = 1f - (dist / explosionRadius);
            rb.AddForce(toBody.normalized * explosionStrength * falloff, ForceMode.Impulse);
        }
    }

    void ResetExplosionImage() => SetSingleImage(explosionImage, false);

    // ─── HELPERS UI — mismo patrón que el original ───────────────────────────

    void setGravityImages(bool v)
    {
        var onColor = gravityOnImage.color;
        onColor.a = v ? .3f : 1f;
        gravityOnImage.color = onColor;

        var offColor = gravityOffImage.color;
        offColor.a = v ? 1f : .3f;
        gravityOffImage.color = offColor;
    }

    void setTimeImage(bool v)
    {
        var timeColor = timeImage.color;
        timeColor.a = v ? 1f : .3f;
        timeImage.color = timeColor;
    }

    void SetImages(Image onImg, Image offImg, bool activo)
    {
        if (onImg != null)
        {
            var c = onImg.color; c.a = activo ? 1f : 0.3f;
            onImg.color = c;
        }
        if (offImg != null)
        {
            var c = offImg.color; c.a = activo ? 0.3f : 1f;
            offImg.color = c;
        }
    }

    void SetSingleImage(Image img, bool activo)
    {
        if (img == null) return;
        var c = img.color; c.a = activo ? 1f : 0.3f;
        img.color = c;
    }
}
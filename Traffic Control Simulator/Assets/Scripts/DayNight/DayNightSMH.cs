using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightSMH : MonoBehaviour
{
    // ============================================================
    // TIME
    // ============================================================

    [Header("TIME")]

    [Tooltip("Current game time: 0-24 hours.")]
    [Range(0f, 24f)]
    [SerializeField] private float timeOfDay = 12f;

    [Tooltip("Automatically advance time.")]
    [SerializeField] private bool animateTime = false;

    [Tooltip("Real seconds for one complete game day.")]
    [Min(1f)]
    [SerializeField] private float fullDayDuration = 600f;


    // ============================================================
    // TIME POINTS
    // ============================================================

    [Header("TIME POINTS")]

    [Range(0f, 24f)]
    [SerializeField] private float dayStart = 6f;

    [Range(0f, 24f)]
    [SerializeField] private float sunsetStart = 18f;

    [Range(0f, 24f)]
    [SerializeField] private float twilightStart = 21f;

    [Range(0f, 24f)]
    [SerializeField] private float nightStart = 24f;


    // ============================================================
    // VOLUME
    // ============================================================

    [Header("VOLUME")]

    [SerializeField] private Volume volume;


    // ============================================================
    // TRANSITION CURVES
    // ============================================================

    [Header("TRANSITION CURVES")]

    [SerializeField]
    private AnimationCurve nightToDayCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private AnimationCurve dayToSunsetCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private AnimationCurve sunsetToTwilightCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


    // ============================================================
    // DAY
    // ============================================================

    [Header("DAY")]

    // Neutral SMH value: (1, 1, 1, 0)
    [SerializeField]
    private Vector4 dayMidtones =
        new Vector4(1f, 1f, 1f, 0f);

    [SerializeField] private float dayTemperature = 0f;
    [SerializeField] private float dayTint = 0f;
    [SerializeField] private Color dayColorFilter = Color.white;

    [Range(-6f, 6f)]
    [SerializeField] private float dayPostExposure = 0f;


    // ============================================================
    // SUNSET
    // ============================================================

    [Header("SUNSET")]

    [SerializeField]
    private Vector4 sunsetMidtones =
        new Vector4(1.02f, 1f, 0.98f, 0f);

    [SerializeField] private float sunsetTemperature = 8f;
    [SerializeField] private float sunsetTint = 1f;

    [SerializeField]
    private Color sunsetColorFilter =
        new Color(1f, 0.97f, 0.92f, 1f);

    [Range(-6f, 6f)]
    [SerializeField] private float sunsetPostExposure = -0.1f;


    // ============================================================
    // TWILIGHT
    // ============================================================

    [Header("TWILIGHT")]

    [SerializeField]
    private Vector4 twilightMidtones =
        new Vector4(0.99f, 1f, 1.02f, 0f);

    [SerializeField] private float twilightTemperature = -3f;
    [SerializeField] private float twilightTint = 0f;

    [SerializeField]
    private Color twilightColorFilter =
        new Color(0.96f, 0.97f, 1f, 1f);

    [Range(-6f, 6f)]
    [SerializeField] private float twilightPostExposure = -0.2f;


    // ============================================================
    // NIGHT
    // ============================================================

    [Header("NIGHT")]

    [SerializeField]
    private Vector4 nightMidtones =
        new Vector4(0.98f, 0.99f, 1.03f, 0f);

    [SerializeField] private float nightTemperature = -8f;
    [SerializeField] private float nightTint = 0f;

    [SerializeField]
    private Color nightColorFilter =
        new Color(0.94f, 0.96f, 1f, 1f);

    [Range(-6f, 6f)]
    [SerializeField] private float nightPostExposure = -0.5f;


    // ============================================================
    // INTERNAL
    // ============================================================

    private ShadowsMidtonesHighlights smh;
    private WhiteBalance whiteBalance;
    private ColorAdjustments colorAdjustments;

    private bool initialized;


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        Initialize();
    }


    // ============================================================
    // INITIALIZE
    // ============================================================

    private void Initialize()
    {
        if (volume == null)
        {
            Debug.LogError(
                "DayNightSMH: Assign a Global Volume in the Inspector."
            );

            enabled = false;
            return;
        }

        // Use the runtime profile instance.
        VolumeProfile profile = volume.profile;

        if (!profile.TryGet(out smh))
        {
            Debug.LogError(
                "DayNightSMH: Shadows Midtones Highlights is missing " +
                "from the Volume Profile."
            );

            enabled = false;
            return;
        }

        if (!profile.TryGet(out whiteBalance))
        {
            Debug.LogError(
                "DayNightSMH: White Balance is missing " +
                "from the Volume Profile."
            );

            enabled = false;
            return;
        }

        if (!profile.TryGet(out colorAdjustments))
        {
            Debug.LogError(
                "DayNightSMH: Color Adjustments is missing " +
                "from the Volume Profile."
            );

            enabled = false;
            return;
        }

        // Ensure the effects and parameters are enabled.
        smh.active = true;
        smh.midtones.overrideState = true;

        whiteBalance.active = true;
        whiteBalance.temperature.overrideState = true;
        whiteBalance.tint.overrideState = true;

        colorAdjustments.active = true;
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.postExposure.overrideState = true;

        initialized = true;

        UpdateColorGrading();

        Debug.Log("DayNightSMH initialized successfully.");
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        if (!initialized)
            return;

        if (animateTime)
        {
            float duration = Mathf.Max(1f, fullDayDuration);
            float hoursPerSecond = 24f / duration;

            timeOfDay = Mathf.Repeat(
                timeOfDay + hoursPerSecond * Time.deltaTime,
                24f
            );
        }

        UpdateColorGrading();
    }


    // ============================================================
    // COLOR GRADING
    // ============================================================

    private void UpdateColorGrading()
    {
        // NIGHT -> DAY: 00:00-06:00
        if (timeOfDay < dayStart)
        {
            float t = Mathf.InverseLerp(
                0f, dayStart, timeOfDay
            );

            t = nightToDayCurve.Evaluate(t);

            ApplyTransition(
                nightMidtones, dayMidtones,
                nightTemperature, dayTemperature,
                nightTint, dayTint,
                nightColorFilter, dayColorFilter,
                nightPostExposure, dayPostExposure,
                t
            );

            return;
        }

        // DAY: 06:00-18:00
        if (timeOfDay < sunsetStart)
        {
            ApplyState(
                dayMidtones,
                dayTemperature,
                dayTint,
                dayColorFilter,
                dayPostExposure
            );

            return;
        }

        // DAY -> SUNSET: 18:00-21:00
        if (timeOfDay < twilightStart)
        {
            float t = Mathf.InverseLerp(
                sunsetStart, twilightStart, timeOfDay
            );

            t = dayToSunsetCurve.Evaluate(t);

            ApplyTransition(
                dayMidtones, sunsetMidtones,
                dayTemperature, sunsetTemperature,
                dayTint, sunsetTint,
                dayColorFilter, sunsetColorFilter,
                dayPostExposure, sunsetPostExposure,
                t
            );

            return;
        }

        // SUNSET -> TWILIGHT: 21:00-24:00
        if (timeOfDay < nightStart)
        {
            float t = Mathf.InverseLerp(
                twilightStart, nightStart, timeOfDay
            );

            t = sunsetToTwilightCurve.Evaluate(t);

            ApplyTransition(
                sunsetMidtones, twilightMidtones,
                sunsetTemperature, twilightTemperature,
                sunsetTint, twilightTint,
                sunsetColorFilter, twilightColorFilter,
                sunsetPostExposure, twilightPostExposure,
                t
            );

            return;
        }

        // Fallback
        ApplyState(
            nightMidtones,
            nightTemperature,
            nightTint,
            nightColorFilter,
            nightPostExposure
        );
    }


    // ============================================================
    // APPLY STATE
    // ============================================================

    private void ApplyState(
        Vector4 midtones,
        float temperature,
        float tint,
        Color colorFilter,
        float postExposure
    )
    {
        smh.midtones.value = midtones;

        whiteBalance.temperature.value = temperature;
        whiteBalance.tint.value = tint;

        colorAdjustments.colorFilter.value = colorFilter;
        colorAdjustments.postExposure.value = postExposure;
    }


    // ============================================================
    // APPLY TRANSITION
    // ============================================================

    private void ApplyTransition(
        Vector4 fromMidtones,
        Vector4 toMidtones,

        float fromTemperature,
        float toTemperature,

        float fromTint,
        float toTint,

        Color fromColorFilter,
        Color toColorFilter,

        float fromExposure,
        float toExposure,

        float t
    )
    {
        t = Mathf.Clamp01(t);

        smh.midtones.value = Vector4.Lerp(
            fromMidtones, toMidtones, t
        );

        whiteBalance.temperature.value = Mathf.Lerp(
            fromTemperature, toTemperature, t
        );

        whiteBalance.tint.value = Mathf.Lerp(
            fromTint, toTint, t
        );

        colorAdjustments.colorFilter.value = Color.Lerp(
            fromColorFilter, toColorFilter, t
        );

        colorAdjustments.postExposure.value = Mathf.Lerp(
            fromExposure, toExposure, t
        );
    }


    // ============================================================
    // PUBLIC API
    // ============================================================

    public void SetTime(float hours)
    {
        timeOfDay = Mathf.Repeat(hours, 24f);

        if (initialized)
            UpdateColorGrading();
    }

    public float GetTime()
    {
        return timeOfDay;
    }

    public void SetAnimateTime(bool value)
    {
        animateTime = value;
    }

    public bool IsNight()
    {
        return timeOfDay < dayStart;
    }

    public bool IsDay()
    {
        return timeOfDay >= dayStart &&
               timeOfDay < sunsetStart;
    }

    public bool IsSunset()
    {
        return timeOfDay >= sunsetStart &&
               timeOfDay < twilightStart;
    }

    public bool IsTwilight()
    {
        return timeOfDay >= twilightStart &&
               timeOfDay < nightStart;
    }
}
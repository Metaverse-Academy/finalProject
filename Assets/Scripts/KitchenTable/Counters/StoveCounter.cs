using System;
using System.Collections;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChange;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fryingSound;    // ’Ê  «·ﬁ·Ì
    [SerializeField] private AudioClip burningSound;   // ’Ê  «·Õ—ﬁ
    [SerializeField] private float soundInterval = 3f; // › —…  ﬂ—«— «·’Ê 

    private Coroutine soundCoroutine;
    private bool isSoundPlaying = false;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurnedRecipeSO[] burnedRecipeSOArray;

    private float fryingTimer;
    private float burnedTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurnedRecipeSO burnedRecipeSO;
    private State state;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                StopSound(); // √Êﬁ› «·’Ê  ⁄‰œ„« ÌﬂÊ‰ idle
                break;

            case State.Frying:
                if (fryingRecipeSO != null)
                {
                    fryingTimer += Time.deltaTime;

                    // ‘€· ’Ê  «·ﬁ·Ì
                    PlaySound(fryingSound);

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.FryingTimeMax
                    });

                    if (fryingTimer > fryingRecipeSO.FryingTimeMax)
                    {
                        // Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        state = State.Fried;
                        burnedTimer = 0f;
                        burnedRecipeSO = GetBurnedRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        // √Êﬁ› ’Ê  «·ﬁ·Ì Ê«»œ√ ’Ê  «·Õ—ﬁ
                        StopSound();
                        PlaySound(burningSound);

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    }
                }
                else
                {
                    state = State.Idle;
                    StopSound();
                }
                break;

            case State.Fried:
                if (burnedRecipeSO != null)
                {
                    burnedTimer += Time.deltaTime;

                    // «” „— ›Ì  ‘€Ì· ’Ê  «·Õ—ﬁ
                    PlaySound(burningSound);

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                    {
                        progressNormalized = burnedTimer / burnedRecipeSO.BurendTimeMax
                    });

                    if (burnedTimer > burnedRecipeSO.BurendTimeMax)
                    {
                        // Burned
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burnedRecipeSO.output, this);
                        state = State.Burned;

                        // √Êﬁ› «·’Ê  ⁄‰œ„« ÌÕ —ﬁ  „«„«
                        StopSound();

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = 0f });
                    }
                }
                break;

            case State.Burned:
                StopSound(); //  √ﬂœ „‰ ≈Ìﬁ«› «·’Ê  ›Ì Õ«·… Burned
                break;
        }
    }

    #region Sound Management
    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        // ≈–« «·’Ê  „« ‘€«·° «»œ√ «· ”·”·
        if (!isSoundPlaying)
        {
            isSoundPlaying = true;
            if (soundCoroutine != null)
                StopCoroutine(soundCoroutine);
            soundCoroutine = StartCoroutine(SoundRoutine(clip));
        }
    }

    private void StopSound()
    {
        isSoundPlaying = false;
        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
            soundCoroutine = null;
        }
        audioSource.Stop();
    }

    private IEnumerator SoundRoutine(AudioClip clip)
    {
        while (isSoundPlaying)
        {
            // ‘€· «·’Ê 
            audioSource.PlayOneShot(clip);

            // «‰ Ÿ— ﬁ»· «· ‘€Ì· «· «·Ì
            yield return new WaitForSeconds(soundInterval);

            //  Õﬁﬁ ≈–« ·« “«· ›Ì ‰›” «·Õ«·…
            if (!isSoundPlaying) break;
        }
    }
    #endregion

    public override void Interact(PlayerMovement player)
    {
        if (!HasKitchenObject())
        {
            // There is no kitchen object on the counter
            if (player.HasKitchenObject())
            {
                // Player is carrying something so put it on the counter
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Player with something that can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;

                    // «»œ√ ’Ê  «·ﬁ·Ì
                    PlaySound(fryingSound);

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                // There is a kitchen object here
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        fryingRecipeSO = null;
                        fryingTimer = 0f;

                        // √Êﬁ› «·’Ê 
                        StopSound();

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = 0f });
                    }
                }
            }
            else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
                // Reset stove state when item is taken
                state = State.Idle;
                fryingRecipeSO = null;
                fryingTimer = 0f;

                // √Êﬁ› «·’Ê 
                StopSound();

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs { progressNormalized = 0f });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO?.output;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurnedRecipeSO GetBurnedRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurnedRecipeSO burnedRecipeSO in burnedRecipeSOArray)
        {
            if (burnedRecipeSO.input == inputKitchenObjectSO)
            {
                return burnedRecipeSO;
            }
        }
        return null;
    }
}
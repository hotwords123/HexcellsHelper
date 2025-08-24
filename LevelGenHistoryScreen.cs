using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexcellsHelper
{
    public class LevelGenHistoryScreen : MonoBehaviour
    {
        public static LevelGenHistoryScreen Instance { get; private set; }

        public static readonly Vector3 OffsetPosition = new(24f, 0f, 0f);
        public static readonly Vector3 OriginalCameraPosition = new(24f, -13f, -10f);
        public static readonly Vector3 CameraPosition = OriginalCameraPosition + OffsetPosition;

        public bool IsScreenActive { get; private set; } = false;

        MusicDirector musicDirector;
        Tween cameraTween;

        GameObject historyEntryPrefab;
        Material completedLevelIcon;

        Transform entryParent;
        Transform endIndicator;
        Transform openerText;
        ScrollBar scrollBar;

        bool wasHardModeActive;

        List<LevelGenHistoryEntry> historyEntries;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0)
            {
                musicDirector = GameObjectUtil.GetMusicDirector();
                cameraTween = GameObjectUtil.GetMainCamera().GetComponent<Tween>();

                if (historyEntryPrefab == null)
                {
                    InitializePrefab();
                }

                InitializeUI();
                ResetHistoryEntries();

                if (IsScreenActive)
                {
                    LoadHistory();
                    GameObjectUtil.GetMainCamera().transform.position = CameraPosition;

                    // Restore the hard mode state when returning from a history level
                    GameObjectUtil.GetOptionsManager().currentOptions.levelGenHardModeActive = wasHardModeActive;
                }
            }
        }

        public void EnterScreen()
        {
            IsScreenActive = true;
            if (historyEntries == null)
            {
                LoadHistory();
            }

            cameraTween.targetPosition = CameraPosition;
            cameraTween.Play();
        }

        public void ExitScreen()
        {
            IsScreenActive = false;

            cameraTween.targetPosition = OriginalCameraPosition;
            cameraTween.Play();
        }

        public void LoadHistoryLevel(LevelGenHistoryEntry entry)
        {
            musicDirector.ChangeTrack((int.Parse(entry.seed) % 9) switch
            {
                0 => MusicDirector.Track.Aeolian,
                1 => MusicDirector.Track.Melodia,
                2 => MusicDirector.Track.Raindrops,
                3 => MusicDirector.Track.M1,
                4 => MusicDirector.Track.M2,
                5 => MusicDirector.Track.M3,
                6 => MusicDirector.Track.NewTrack1,
                7 => MusicDirector.Track.NewTrack2,
                8 => MusicDirector.Track.NewTrack3,
                _ => musicDirector.currentTrack,
            });

            var gameManager = GameObjectUtil.GetGameManager();
            gameManager.seedNumber = entry.seed;
            gameManager.isLoadingSavedLevelGenState = false;

            var currentOptions = GameObjectUtil.GetOptionsManager().currentOptions;
            wasHardModeActive = currentOptions.levelGenHardModeActive;
            currentOptions.levelGenHardModeActive = entry.difficulty == "hard";

            GameObject.Find("Fader").GetComponent<FaderScript>().FadeOut(37);
            GameObject.Find("Loading Text").GetComponent<LoadingText>().FadeIn();
        }

        void InitializePrefab()
        {
            var customLevelManager = GameObjectUtil.GetCustomLevelManager();

            historyEntryPrefab = Instantiate(customLevelManager.redditLevelPrefab);
            historyEntryPrefab.name = "History Entry";
            historyEntryPrefab.SetActive(false);
            DontDestroyOnLoad(historyEntryPrefab);

            // Set the prefab to be used for history entries
            var background = historyEntryPrefab.transform.Find("Background");
            var origItem = background.GetComponent<RedditListObject>();
            var item = background.gameObject.AddComponent<LevelGenHistoryItem>();
            item.mouseOff = origItem.mouseOff;
            item.mouseOffAlternate = origItem.mouseOffTigerDark;
            item.mouseOver = origItem.mouseOver;
            item.border = origItem.border;
            item.borderHighlight = origItem.borderHighlighted;
            DestroyImmediate(origItem);

            // Remove unnecessary components
            DestroyImmediate(historyEntryPrefab.transform.Find("Score").gameObject);
            DestroyImmediate(historyEntryPrefab.transform.Find("Comments").gameObject);
            DestroyImmediate(historyEntryPrefab.transform.Find("Hyperlink").gameObject);
            DestroyImmediate(historyEntryPrefab.transform.Find("Save State Icon").gameObject);

            completedLevelIcon = customLevelManager.completedLevelIconMat;
        }

        void InitializeUI()
        {
            var customLevelsScreen = GameObject.Find("Custom Levels Screen").transform;
            var levelGeneratorScreen = GameObject.Find("Level Generator Screen").transform;

            // Create the title text for the history screen
            var origTitle = levelGeneratorScreen.Find("Level Generator Label");
            var titleGO = Instantiate(origTitle.gameObject, levelGeneratorScreen);
            titleGO.name = "History Title";
            titleGO.transform.position += OffsetPosition;
            titleGO.GetComponent<LoadLocalizedText>().textToRetrieveID = "PUZZLEGENERATOR_COMPLETED";

            // Create the history container
            var origContainer = customLevelsScreen.Find("Stencil Write BG");
            var containerGO = Instantiate(origContainer.gameObject, levelGeneratorScreen);
            containerGO.name = "History Container";
            containerGO.transform.position += OffsetPosition;

            var scrollParent = containerGO.transform.Find("User Levels Scroll Parent");
            scrollParent.name = "History Scroll Parent";

            entryParent = scrollParent.transform.Find("Reddit List Objects");
            entryParent.name = "History Entries";

            endIndicator = scrollParent.Find("Load more levels button");
            endIndicator.name = "History End Indicator";
            endIndicator.GetComponent<TextMeshPro>().text = "- End -";
            DestroyImmediate(endIndicator.GetComponent<BoxCollider>());
            DestroyImmediate(endIndicator.GetComponent<LoadMoreRedditLevelsButton>());
            DestroyImmediate(endIndicator.GetComponent<LoadLocalizedText>());

            // Create the scroll bar
            var origScrollBarLine = customLevelsScreen.Find("Scroll Bar Line");
            var scrollBarLineGO = Instantiate(origScrollBarLine.gameObject, levelGeneratorScreen);
            scrollBarLineGO.name = "History Scroll Bar Line";
            scrollBarLineGO.transform.position += OffsetPosition;

            var origScrollBarButton = GameObject.Find("Scroll Bar Button").transform;
            var scrollBarButtonGO = Instantiate(
                origScrollBarButton.gameObject,
                scrollBarLineGO.transform.position + (origScrollBarButton.position - origScrollBarLine.position),
                origScrollBarButton.rotation
            );
            scrollBarButtonGO.name = "History Scroll Bar Button";

            scrollBar = scrollBarButtonGO.GetComponent<ScrollBar>();
            AccessUtil.GetFieldInfo<ScrollBar>("userLevelsScrollParent").SetValue(scrollBar, scrollParent);

            // Open the history when clicking on the "Puzzles Completed" text
            openerText = levelGeneratorScreen.Find("Puzzle Completed Text");
            openerText.gameObject.AddComponent<LevelGenHistoryOpener>();
            StartCoroutine(AdjustOpenerCollider());

            // Create the back button to return to the level generator
            var origBackButton = levelGeneratorScreen.Find("Back Button - Generator");
            var backButtonGO = Instantiate(origBackButton.gameObject, levelGeneratorScreen);
            backButtonGO.name = "Back Button - History";
            backButtonGO.transform.position += OffsetPosition;
            backButtonGO.AddComponent<LevelGenHistoryBackButton>();
            DestroyImmediate(backButtonGO.GetComponent<MenuToTitleButton>());
        }

        IEnumerator AdjustOpenerCollider()
        {
            yield return new WaitForEndOfFrame(); // Wait for the text to be rendered

            var openerBounds = openerText.GetComponent<TextMeshPro>().GetRenderedValues();
            var openerCollider = openerText.gameObject.AddComponent<BoxCollider>();
            openerCollider.size = new Vector3(openerBounds.x, openerBounds.y, 0.01f);
        }

        void LoadHistory()
        {
            historyEntries = LevelGenHistoryManager.LoadHistory();
            // Reverse the order to show the most recent first
            historyEntries.Reverse();

            RenderHistoryEntries();
        }

        void RenderHistoryEntries()
        {
            var solvedPuzzleIds = new HashSet<string>();

            for (var i = historyEntries.Count - 1; i >= 0; i--)
            {
                var entry = historyEntries[i];

                var entryGO = Instantiate(historyEntryPrefab, entryParent);
                entryGO.name = $"History Entry {i + 1}";
                entryGO.transform.localPosition = new Vector3(-6.3f, 3.45f - i * 0.7f, 0.05f);
                entryGO.SetActive(true);

                var puzzleId = $"{entry.difficulty}-{entry.seed}";
                var isNewlySolved = solvedPuzzleIds.Add(puzzleId);
                if (isNewlySolved)
                {
                    entryGO.GetComponent<Renderer>().material = completedLevelIcon;
                }

                var item = entryGO.transform.Find("Background").GetComponent<LevelGenHistoryItem>();
                item.entry = entry;
                item.SetAlternate(i % 2 == 1);

                var datetime = DateTimeOffset.Parse(entry.timestamp);
                var timeSpan = TimeSpan.FromSeconds(entry.timeTaken);

                var seedText = entry.seed == datetime.ToString("ddMMyyyy")
                    ? $"<color=#06A4EA>{entry.seed}</color>"
                    : entry.seed;
                var levelName = $"{entry.difficulty.Capitalize()} - {seedText}";
                entryGO.transform.Find("Level Name").GetComponent<TextMeshPro>().text = levelName;

                var details = new string[][]
                {
                    ["Completed at", datetime.ToString("yyyy-MM-dd HH:mm:ss")],
                    ["Time Taken", $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}"],
                    ["Mistakes", entry.mistakes.ToString()],
                };
                var detailText = string.Join("\t\t", details.Select(d =>
                    $"<color=#CCCCCC>{d[0]}</color> <color=#969696>{d[1]}</color>").ToArray());
                entryGO.transform.Find("Tags").GetComponent<TextMeshPro>().text = detailText;
            }

            endIndicator.localPosition = new Vector3(0.0f, 3.45f - historyEntries.Count * 0.7f - 0.07f, 0.05f);
            endIndicator.gameObject.SetActive(true);
            scrollBar.UpdateNumberOfLevelsInList(historyEntries.Count);
        }

        void ResetHistoryEntries()
        {
            foreach (Transform child in entryParent)
            {
                Destroy(child.gameObject);
            }

            endIndicator.gameObject.SetActive(false);
            scrollBar.UpdateNumberOfLevelsInList(0);
            historyEntries = null;
        }
    }
}

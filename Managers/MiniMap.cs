using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZB.Gameplay;

namespace ZB.UI
{
    public class MiniMap : MonoBehaviour
    {
        protected static readonly float checkPositionDelay = 0.05f;
        [SerializeField] GameObject playerOnMap;
        [SerializeField] GameObject chestOnMap;
        [SerializeField] float scale = 1f;
        [SerializeField] float widthFrame = 300f;
        [SerializeField] float heightFrame = 300f;
        [SerializeField] private AvatarHeroMiniMap[] avatarHeroes;
        [SerializeField] private Color green = new Color32(8, 246, 8, 225);
        [SerializeField] private Color blue = new Color32(10, 168, 247, 225);
        [SerializeField] private Color red = new Color32(230, 22, 17, 255);
        private bool isUpdate = true;

        private List<Transform> entityTransform = new List<Transform>();
        private List<RectTransform> enemiesOnMap = new List<RectTransform>();
        private List<GameObject> chestPointOnMapObj = new List<GameObject>();
        private void Awake()
        {
            EventManager.AddListener<CharacterModel>(GameEventType.SpawnHero, OnSpawnHero);
            EventManager.AddListener<CharacterModel>(GameEventType.HeroDie, OnHeroDie);
            EventManager.AddListener<CharacterModel>(GameEventType.SpawnEnemy, OnSpawnEnemy);
            EventManager.AddListener<CharacterModel>(GameEventType.EnemyDie, OnEnemyDie);
            EventManager.AddListener<Vector3>(GameEventType.SpawnChest, OnSpawnChest);
            EventManager.AddListener<Vector3>(GameEventType.DestroyChest, OnDestroyChest);
            EventManager.AddListener(GameEventType.WinGame, OnWinGame);
            EventManager.AddListener(GameEventType.LoseGame, OnLoseGame);
            EventManager.AddListener(GameEventType.PauseGame, OnPauseGame);
            EventManager.AddListener(GameEventType.ContinueGame, OnContinueGame);
        }

        protected virtual void Start()
        {
            StartCoroutine(CheckPosition());
        }

        // void Update()
        // {
        //     for (int i = 0; i < entityTransform.Count; i++)
        //     {
        //         AlignPosition(i);
        //     }
        // }

        protected virtual IEnumerator CheckPosition()
        {
            while (true)
            {
                yield return new WaitForSeconds(checkPositionDelay);
                if (isUpdate)
                {
                    if (enemiesOnMap.Count > 0)
                    {
                        enemiesOnMap.ForEach(item => item.gameObject.SetActive(false));
                    }
                    for (int i = 0; i < entityTransform.Count; i++)
                    {
                        AlignPosition(i);
                    }
                    
                }
            }
        }

        private void OnPauseGame()
        {
            isUpdate = false;
        }

        private void OnContinueGame()
        {
            isUpdate = true;
        }

        private void OnWinGame()
        {
            isUpdate = false;
        }

        private void OnLoseGame()
        {
            isUpdate = false;
        }

        private void OnSpawnChest(Vector3 chestPoint)
        {
            GameObject temp = Instantiate(chestOnMap);
            temp.transform.SetParent(transform);
            temp.transform.gameObject.SetActive(true);
            temp.GetComponent<RectTransform>().localPosition = new Vector2(chestPoint.x * widthFrame/100, chestPoint.z * heightFrame/100) * scale;
            chestPointOnMapObj.Add(temp);
        }

        private void OnDestroyChest(Vector3 chestPoint)
        {
            GameObject chestFind = chestPointOnMapObj.Find(item => item.GetComponent<RectTransform>().localPosition.x / (widthFrame/100) / scale == chestPoint.x && item.GetComponent<RectTransform>().localPosition.y / (heightFrame/100) / scale == chestPoint.z);
            if (chestFind != null)
            {
                chestPointOnMapObj.Remove(chestFind);
                Destroy(chestFind);
            }
        }

        private void OnSpawnHero(CharacterModel heroModel)
        {
            setSpawner(heroModel);
        }

        private void OnHeroDie(CharacterModel heroModel)
        {
            entityTransform.Remove(heroModel.ownerTransform);
        }

        private void OnSpawnEnemy(CharacterModel enemyModel)
        {
            setSpawner(enemyModel);
        }

        private void OnEnemyDie(CharacterModel enemyModel)
        {
            entityTransform.Remove(enemyModel.ownerTransform);
        }

        private void setSpawner(CharacterModel spawnerModel)
        {
            entityTransform.Add(spawnerModel.ownerTransform);
            GameObject temp = Instantiate(playerOnMap);
            temp.transform.SetParent(transform);
            temp.transform.localScale = Vector3.one;
            enemiesOnMap.Add(temp.GetComponent<RectTransform>());
            Sprite avatarSprite = avatarHeroes.First(x => x.characterType == spawnerModel.CharacterType).avatarSprite;
            temp.transform.GetChild(0).GetComponent<Image>().sprite = avatarSprite;
            if (spawnerModel.IsEnemy)
            {
                temp.GetComponent<Image>().color = red;
            }
            else
            {
                if (spawnerModel.IsControlled)
                {
                    temp.GetComponent<Image>().color = green;
                }
                else
                {
                    temp.GetComponent<Image>().color = blue;
                }
            }
        }

        void AlignPosition(int i)
        {
            enemiesOnMap[i].localPosition = new Vector2(entityTransform[i].position.x * widthFrame/100, entityTransform[i].position.z * heightFrame/100) * scale;
            enemiesOnMap[i].gameObject.SetActive(true);
        }
    }
    [Serializable]
    public struct AvatarHeroMiniMap
    {
        public CharacterType characterType;
        public Sprite avatarSprite;
    }
}

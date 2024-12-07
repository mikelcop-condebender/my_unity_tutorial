using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
   public static GameManager instance;
   public int MaxNumberOfShots = 3;
   [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f; 
   [SerializeField] private GameObject _restartGameObject;
   [SerializeField] private SlingShotHandler _slingShotHandler;
   private int _usedNumberOfShots;
   private IconHandler _iconHandler;
   private List<Baddie> _baddies = new List<Baddie>();

   private void Awake()
   {
      if(instance == null)
      {
         instance = this;
      }
      _iconHandler = FindAnyObjectByType<IconHandler>();

      Baddie[] baddies = FindObjectsByType<Baddie>(FindObjectsSortMode.None);
        // Log the number of baddies found
      for (int i = 0; i < baddies.Length; i++)
      {
         _baddies.Add(baddies[i]);
      }
   }

   public void UseShot(){
    _usedNumberOfShots++;
    _iconHandler.UseShot(_usedNumberOfShots);
    CheckForLastShot();
   }

   public bool HasEnoughShot()
   {
    return _usedNumberOfShots < MaxNumberOfShots;
   }

   private void CheckForLastShot(){
      if(_usedNumberOfShots == MaxNumberOfShots)
      {
         StartCoroutine(CheckAfterWaitTime());
      }
   }

   private IEnumerator CheckAfterWaitTime(){
      yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);
      if(_baddies.Count == 0)
      {
         WinGame();
      }else{
         RestartGame();
      }
   }

   public void RemoveBaddie(Baddie baddie)
   {
      _baddies.Remove(baddie);
      CheckForAllDeadBaddies();
   }

   private void CheckForAllDeadBaddies()
   {
      if(_baddies.Count == 0)
      {
         WinGame();
      }
   }

   private void WinGame()
   {
      _restartGameObject.SetActive(true);
      _slingShotHandler.enabled = false;
   }

   private void RestartGame()
   {
   Debug.Log("Lose Game");
   SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }
}

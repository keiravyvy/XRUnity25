using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DialougeManager : MonoBehaviour
{
    public TMP_Text dialougeText;
    public TMP_Text nextText;
    public String[] dialouges;
    
    // Add reference for the input action
    public InputActionReference nextDialogueAction;
    public float delayTime = 10f;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private bool dialogueStarted = false;
    
    // Speed of the typewriter effect (lower = slower)
    public float typingSpeed = 0.05f;
    
    void Start()
    {
        // Clear text at the beginning
        dialougeText.text = "";
        
        if (nextText != null)
            nextText.gameObject.SetActive(false);
        
        // Enable input action
        if (nextDialogueAction != null)
        {
            nextDialogueAction.action.Enable();
            nextDialogueAction.action.performed += OnNextDialogue;
        }
        
        // Start the delay coroutine
        StartCoroutine(StartDialogueAfterDelay(delayTime));
    }
    
    private IEnumerator StartDialogueAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        // Start the dialogue
        dialogueStarted = true;
        DisplayNextDialogue();
    }
    
    private void OnNextDialogue(InputAction.CallbackContext context)
    {
        // Only respond if dialogue has started
        if (!dialogueStarted)
            return;
        
        // If currently typing, complete the current text immediately
        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
            
            // Show full text immediately
            if (currentDialogueIndex < dialouges.Length)
                dialougeText.text = dialouges[currentDialogueIndex];
                
            // Show next prompt
            if (nextText != null)
                nextText.gameObject.SetActive(true);
        }
        // Otherwise, move to the next dialogue
        else
        {
            currentDialogueIndex++;
            DisplayNextDialogue();
        }
    }
    
    private void DisplayNextDialogue()
    {
        // Hide the next prompt while typing
        if (nextText != null)
            nextText.gameObject.SetActive(false);
        
        // Check if we have more dialogues to display
        if (currentDialogueIndex < dialouges.Length)
        {
            // Start typewriter effect
            StartCoroutine(TypeWriterEffect(dialouges[currentDialogueIndex]));
        }
        else
        {
            // No more dialogues, load the game scene
            SceneManager.LoadScene("GameScene");
        }
    }
    
    private IEnumerator TypeWriterEffect(string text)
    {
        isTyping = true;
        dialougeText.text = "";
        
        // Display each character one by one
        foreach (char c in text.ToCharArray())
        {
            dialougeText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        
        // Show next prompt after typing is complete
        if (nextText != null)
            nextText.gameObject.SetActive(true);
    }
    
    void OnDestroy()
    {
        // Clean up input action binding
        if (nextDialogueAction != null)
        {
            nextDialogueAction.action.performed -= OnNextDialogue;
            nextDialogueAction.action.Disable();
        }
    }
}
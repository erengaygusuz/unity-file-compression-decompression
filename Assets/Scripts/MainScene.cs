using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI compressionInfoTxt;
    [SerializeField] private TextMeshProUGUI decompressionInfoTxt;
    [SerializeField] private Button compressionTryAgainBtn;
    [SerializeField] private Button decompressionTryAgainBtn;

    private string compressionSavePath, filesPathToBeCompress;
    private string decompressionSavePath, filesPathToBeDecompress;

    private void Start()
    {
        PathAssigning();
        EventBinding();
    }

    private void PathAssigning()
    {
        filesPathToBeCompress = @"C:\Users\User\Desktop\NewFolder";
        compressionSavePath = @"C:\Users\User\Desktop\CompressionSave";

        filesPathToBeDecompress = @"C:\Users\User\Desktop\CompressionSave";
        decompressionSavePath = @"C:\Users\User\Desktop\DecompressionSave";
    }

    private void EventBinding()
    {
        FileProcessManager.Compression.Started = new GenericEvent();
        FileProcessManager.Compression.Started.AddListener(() => {

            compressionInfoTxt.text = "Compression process is started.";
            Debug.Log("Compression is started.");

        });

        FileProcessManager.Compression.Continue = new GenericEvent();
        FileProcessManager.Compression.Continue.AddListener(() => {

            compressionInfoTxt.text = "Compression: " + FileProcessManager.Compression.ActiveFileName + "\n" + FileProcessManager.Compression.PercentageOfProgression + "% Completed";
            Debug.Log("Compression is continue.");

        });

        FileProcessManager.Compression.Finished = new GenericEvent();
        FileProcessManager.Compression.Finished.AddListener(() => {

            compressionInfoTxt.text = "Compression: " + FileProcessManager.Compression.ActiveFileName + "\n" + "successfully completed";
            Debug.Log("Compression is finished.");

        });

        FileProcessManager.Decompression.Started = new GenericEvent();
        FileProcessManager.Decompression.Started.AddListener(() => {

            decompressionInfoTxt.text = "Decompression process is started.";
            Debug.Log("Decompression is started.");

        });

        FileProcessManager.Decompression.Continue = new GenericEvent();
        FileProcessManager.Decompression.Continue.AddListener(() => {

            decompressionInfoTxt.text = "Decompression: " + FileProcessManager.Decompression.ActiveFileName + "\n" + FileProcessManager.Decompression.PercentageOfProgression + "% Completed";
            Debug.Log("Decompression is continue.");

        });

        FileProcessManager.Decompression.Finished = new GenericEvent();
        FileProcessManager.Decompression.Finished.AddListener(() => {

            decompressionInfoTxt.text = "Decompression: " + FileProcessManager.Decompression.ActiveFileName + "\n" + "successfully completed";
            Debug.Log("Decompression is finished.");

        });
    }

    public void CompressionStartBtnClick()
    {
        FileProcessManager.Compression.FilesFolderPath = filesPathToBeCompress;
        FileProcessManager.Compression.TargetSavePath = compressionSavePath;
        FileProcessManager.Compression.FileExtensionList = new List<string> { ".txt", ".jpg" };

        try
        {
            FileProcessManager.Compression.Start(() => { AfterCompressionProcess(); } );
        }

        catch (FileProcessManager.SamePathException ex)
        {
            compressionInfoTxt.text = "Exception: " + ex.Message;
            compressionTryAgainBtn.gameObject.SetActive(true);
        }

        catch (FileProcessManager.EmptyPathNameException ex)
        {
            compressionInfoTxt.text = "Exception: " + ex.Message;
            compressionTryAgainBtn.gameObject.SetActive(true);
        }

        catch (Exception ex)
        {
            compressionInfoTxt.text = "Exception: Compression process is unsuccessfull.\n Please click Try Again button.";
            compressionTryAgainBtn.gameObject.SetActive(true);
        }
    }

    public void DecompressionStartBtnClick()
    {
        FileProcessManager.Decompression.FilesFolderPath = filesPathToBeDecompress;
        FileProcessManager.Decompression.TargetSavePath = decompressionSavePath;
        FileProcessManager.Decompression.DeleteFilesAfterDecompress = false;

        try
        {
            FileProcessManager.Decompression.Start(() => { AfterDecompressionProcess(); } );
        }

        catch (FileProcessManager.SamePathException ex)
        {
            decompressionInfoTxt.text = "Exception: " + ex.Message;
            decompressionTryAgainBtn.gameObject.SetActive(true);
        }

        catch (FileProcessManager.EmptyPathNameException ex)
        {
            decompressionInfoTxt.text = "Exception: " + ex.Message;
            decompressionTryAgainBtn.gameObject.SetActive(true);
        }

        catch (Exception ex)
        {
            decompressionInfoTxt.text = "Exception: Decompression process is unsuccessfull.\n Please click Try Again button.";
            decompressionTryAgainBtn.gameObject.SetActive(true);
        }
    }

    private void AfterCompressionProcess()
    {
        compressionInfoTxt.text = "Compression finished successfully.";
        Debug.Log("Compression finished successfully.");
    }

    private void AfterDecompressionProcess()
    {
        decompressionInfoTxt.text = "Decompression finished successfully.";
        Debug.Log("Decompression finished successfully.");
    }
}
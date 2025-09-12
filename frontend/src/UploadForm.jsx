import React, { useState } from 'react';
import axios from 'axios';
import './UploadForm.css';
import CodeDisplay from './CodeDisplay';

const UploadForm = () => {
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [uploadStatus, setUploadStatus] = useState(null);
  const [progress, setProgress] = useState(0);
  const [dragActive, setDragActive] = useState(false);
  const [code, setCode] = useState("");

  const MAX_FILE_SIZE = 10 * 1024 * 1024;

  const handleFileSelect = (selectedFile) => {
    if (!selectedFile) return;

    if (selectedFile.type !== 'application/pdf') {
      setUploadStatus({ type: 'error', message: 'Please select a PDF file only.' });
      setTimeout(() => setUploadStatus(null), 4000);
      return;
    }

    if (selectedFile.size > MAX_FILE_SIZE) {
      setUploadStatus({ type: 'error', message: 'File size too large. Please select a file smaller than 10MB.' });
      setTimeout(() => setUploadStatus(null), 4000);
      return;
    }

    setFile(selectedFile);
    setUploadStatus(null);
    setProgress(0);
  };

  const handleFileChange = (e) => handleFileSelect(e.target.files[0]);

  const handleDrag = (e) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === 'dragenter' || e.type === 'dragover') setDragActive(true);
    else if (e.type === 'dragleave') setDragActive(false);
  };

  const handleDrop = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    if (e.dataTransfer.files && e.dataTransfer.files[0]) handleFileSelect(e.dataTransfer.files[0]);
  };

  const handleBrowseClick = () => {
    document.getElementById('pdf-upload').click();
  };

  const handleUpload = async () => {
    if (!file) return;
    setUploading(true);
    setProgress(0);

    const progressInterval = setInterval(() => {
      setProgress((prev) => {
        if (prev >= 90) {
          clearInterval(progressInterval);
          return 90;
        }
        return prev + Math.random() * 15;
      });
    }, 300);

    try {
      const formData = new FormData();
      formData.append('pdf', file);

      const response = await axios.post(
        'https://awaazbackend.onrender.com/api/upload-presentation',
        formData,
        {
          headers: {},
          onUploadProgress: (progressEvent) => {
            const percentCompleted = Math.round(
              (progressEvent.loaded * 100) / progressEvent.total
            );
            setProgress(percentCompleted);
          },
        }
      );

      clearInterval(progressInterval);
      setProgress(100);
      setUploadStatus({
        type: 'success',
        message: 'Upload successful! PDF uploaded to database.',
      });
      console.log('Upload response:', response.data);

      // Extract code from response and set it
      if (response.data && response.data.code) {
        setCode(response.data.code);
      } else {
        setCode("");
      }
    } catch (error) {
      clearInterval(progressInterval);
      setUploadStatus({
        type: 'error',
        message: 'Upload failed. Please try again.',
      });
      setCode("");
      console.error('Upload error:', error);
    } finally {
      setUploading(false);
    }
  };

  const formatFileSize = (bytes) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  return (
    <div className="upload-form-container">
      <div className="upload-form-header">
        <h2>Upload Your Presentation</h2>
        <p>Get started with VR training by uploading your PDF</p>
      </div>
      <div className="upload-card">
        <div
          className={`upload-dropzone ${
            dragActive ? 'drag-active' : file ? 'file-selected' : ''
          }`}
          onDragEnter={handleDrag}
          onDragLeave={handleDrag}
          onDragOver={handleDrag}
          onDrop={handleDrop}
          onClick={handleBrowseClick}
          role="button"
          tabIndex={0}
        >
          <input
            type="file"
            id="pdf-upload"
            accept=".pdf,application/pdf"
            onChange={handleFileChange}
            className="upload-input"
            disabled={uploading}
          />

          {file ? (
            <div className="file-info" onClick={(e) => e.stopPropagation()}>
              <div className="file-icon">
                <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                  />
                </svg>
              </div>
              <div>
                <p className="file-name">{file.name}</p>
                <p className="file-size">{formatFileSize(file.size)}</p>
              </div>
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  handleBrowseClick();
                }}
                className="change-file-btn"
                disabled={uploading}
              >
                Change File
              </button>
            </div>
          ) : (
            <div className="drop-placeholder">
              <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
                />
              </svg>
              <p>Click here or drag a PDF to upload</p>
              <p className="hint-text">Maximum size: 10MB</p>
            </div>
          )}
        </div>

        {uploading && (
          <div className="progress-section">
            <div className="progress-header">
              <span>Uploading to database...</span>
              <span>{Math.round(progress)}%</span>
            </div>
            <div className="progress-bar-bg">
              <div
                className="progress-bar-fill"
                style={{ width: `${progress}%` }}
              ></div>
            </div>
          </div>
        )}

        {uploadStatus && (
          <div className={`status-message ${uploadStatus.type}`}>
            {uploadStatus.type === 'success' ? (
              <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M5 13l4 4L19 7"
                />
              </svg>
            ) : (
              <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            )}
            <span>{uploadStatus.message}</span>
          </div>
        )}

        {/* Show fancy CodeDisplay instead of inline code box */}
        {code && <CodeDisplay code={code} />}

        <div className="upload-button-wrapper">
          <button
            onClick={handleUpload}
            disabled={!file || uploading}
            className={`upload-button ${
              file && !uploading ? 'active' : 'disabled'
            }`}
          >
            {uploading ? 'Uploading...' : 'Upload Presentation'}
          </button>
        </div>

        <div className="upload-info">
          <p>
            Your PDF will be processed and used to create an immersive VR
            presentation practice session.
          </p>
        </div>
      </div>
    </div>
  );
};

export default UploadForm;

import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './Header';
import MainContent from './MainContent';
import UploadForm from './UploadForm';
import CodeDisplay from './CodeDisplay';
import ReportInput from './ReportInput';
import Report from './report';
import Footer from './Footer';
import About from './About';
import './App.css';

function App() {
  const [generatedCode, setGeneratedCode] = useState('');

  const handleCodeGeneration = (code) => {
    setGeneratedCode(code);
  };

  return (
    <Router>
      <div className="app-container">
        <Header />
        <Routes>
          <Route path="/" element={
            <main className="app-main">
              <div className="main-layout">
                <MainContent />
                <div className="upload-section">
                  {!generatedCode ? (
                    <UploadForm onCodeGeneration={handleCodeGeneration} />
                  ) : (
                    <CodeDisplay code={generatedCode} />
                  )}
                </div>
              </div>
            </main>
          } />
          <Route path="/report-input" element={<ReportInput />} />
          <Route path="/report" element={<Report />} />
          <Route path="/about" element={<About />} />'
        </Routes>
        <Footer />
      </div>
    </Router>
  );
}

export default App;
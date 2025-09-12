import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './reportInput.css';

const LoadingSpinner = () => (
  <div className="loading-spinner"></div>
);

const ReportInput = () => {
  const [code, setCode] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (code.length !== 5) {
      setError('Please enter a 5-digit code');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const response = await fetch(`https://awaazbackend.onrender.com/api/report/${code}`);
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      const data = await response.json();
      
      if (data.success && data.report) {
        navigate('/report', { state: { reportData: data.report } });
      } else {
        setError('Invalid code or no report found. Please try again.');
      }
    } catch (error) {
      console.error('Error fetching report:', error);
      setError('Error fetching report. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="report-input-container">
      <div className="report-input-box">
        {isLoading && (
          <div className="loading-overlay">
            <LoadingSpinner />
          </div>
        )}
        <h2>Enter Report Code</h2>
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              type="text"
              value={code}
              onChange={(e) => setCode(e.target.value.slice(0, 5))}
              placeholder="Enter 5-digit code"
              maxLength="5"
              pattern="[0-9]{5}"
              disabled={isLoading}
            />
          </div>
          {error && <div className="error-message">{error}</div>}
          <button type="submit" disabled={isLoading}>
            <div className="button-content">
              <span>View Report</span>
              {isLoading && <LoadingSpinner />}
            </div>
          </button>
        </form>
      </div>
    </div>
  );
};

export default ReportInput;

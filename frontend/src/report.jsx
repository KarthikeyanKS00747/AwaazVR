import React from 'react';
import { useLocation, Navigate } from 'react-router-dom';
import './report.css';

const Report = () => {
  const location = useLocation();
  const reportData = location.state?.reportData;

  if (!reportData) {
    return <Navigate to="/report-input" replace />;
  }

  return (
    <div className="report-container">
      <div className="report-header">
        <h1>{reportData.title}</h1>
        <p>Report Code: {reportData.code}</p>
      </div>

      <div className="report-summary">
        <h2>Summary</h2>
        <div className="summary-grid">
          <div className="summary-item">
            <span className="label">Pass Rate</span>
            <span className="value">{reportData.summary.passRate}%</span>
          </div>
          <div className="summary-item">
            <span className="label">Average Score</span>
            <span className="value">{reportData.summary.averageScore}%</span>
          </div>
          <div className="summary-item">
            <span className="label">Total Questions</span>
            <span className="value">{reportData.summary.totalQuestions}</span>
          </div>
          <div className="summary-item">
            <span className="label">Passed Questions</span>
            <span className="value">{reportData.summary.passedQuestions}</span>
          </div>
        </div>
      </div>

      <div className="evaluations">
        <h2>Question Evaluations</h2>
        {reportData.evaluations.map((evaluation, index) => (
          <div key={index} className="evaluation-card">
            <h3>Question {index + 1}</h3>
            <p className="question">{evaluation.Question}</p>
            
            <div className="answer-section">
              <h4>User Answer</h4>
              <p>{evaluation["User Answer"]}</p>
            </div>

            {evaluation["Reference Answer"] !== "Unable to generate reference answer" && (
              <div className="answer-section">
                <h4>Reference Answer</h4>
                <p>{evaluation["Reference Answer"]}</p>
              </div>
            )}

            <div className="evaluation-footer">
              <span className="score">
                Similarity Score: {evaluation["Similarity Score"]}%
              </span>
              {evaluation["Missing Points"] !== "Evaluation failed" && (
                <div className="missing-points">
                  <h4>Missing Points</h4>
                  <p>{evaluation["Missing Points"]}</p>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>

      <div className="report-footer">
        <p>Generated at: {new Date(reportData.generatedAt).toLocaleString()}</p>
      </div>
    </div>
  );
};


export default Report;
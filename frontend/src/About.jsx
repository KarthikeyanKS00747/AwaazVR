import React from 'react';

const About = () => {
    const containerStyle = {
        padding: '2rem',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: 'calc(100vh - 80px)',
        position: 'relative',
        zIndex: 1
    };

    const contentStyle = {
        width: '100%',
        maxWidth: '1200px',
        textAlign: 'center'
    };

    const headerStyle = {
        fontSize: '3rem',
        fontWeight: '700',
        color: '#ffffff',
        marginBottom: '1rem',
        textShadow: '0 0 5px #00d9ff, 0 0 10px #00d9ff, 0 0 10px #00d9ff'
    };

    const subtitleStyle = {
        fontSize: '1.2rem',
        color: '#a7cde8',
        marginBottom: '3rem',
        lineHeight: '1.6',
        maxWidth: '800px',
        margin: '0 auto 4rem'
    };

    const featureGridStyle = {
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))',
        gap: '2rem',
        marginBottom: '4rem'
    };

    const featureBoxStyle = {
        background: 'rgba(0, 217, 255, 0.05)',
        border: '1px solid rgba(0, 217, 255, 0.2)',
        borderRadius: '10px',
        padding: '2rem',
        transition: 'all 0.3s ease',
        cursor: 'pointer'
    };

    const featureBoxHoverStyle = {
        ...featureBoxStyle,
        background: 'rgba(0, 217, 255, 0.1)',
        borderColor: 'rgba(0, 217, 255, 0.5)',
        transform: 'translateY(-2px)'
    };

    const featureTitleStyle = {
        fontSize: '1.4rem',
        fontWeight: '600',
        color: '#ffffff',
        marginBottom: '1rem'
    };

    const featureTextStyle = {
        color: '#a7cde8',
        lineHeight: '1.6',
        fontSize: '1rem'
    };

    const missionSectionStyle = {
        background: 'rgba(0, 217, 255, 0.05)',
        border: '1px solid rgba(0, 217, 255, 0.2)',
        borderRadius: '10px',
        padding: '3rem',
        marginBottom: '4rem',
        transition: 'all 0.3s ease'
    };

    const missionTitleStyle = {
        fontSize: '2rem',
        fontWeight: '600',
        color: '#ffffff',
        marginBottom: '1.5rem',
        textShadow: '0 0 5px #00d9ff'
    };

    const missionTextStyle = {
        color: '#a7cde8',
        lineHeight: '1.8',
        fontSize: '1.1rem',
        maxWidth: '800px',
        margin: '0 auto'
    };

    const contactLinkStyle = {
        color: '#00d9ff',
        textDecoration: 'none',
        transition: 'all 0.3s ease',
        ':hover': {
            textShadow: '0 0 8px #00d9ff'
        }
    };

    return (
        <div style={containerStyle}>
            <div style={contentStyle}>
                <h1 style={headerStyle}>About AWAAZ</h1>
                <p style={subtitleStyle}>
                    Transform your presentations into engaging audio-visual experiences with our innovative VR and AI-powered platform.
                </p>

                <div style={missionSectionStyle}>
                    <h2 style={missionTitleStyle}>Our Mission</h2>
                    <p style={missionTextStyle}>
                        We empower individuals and organizations to overcome presentation anxiety and enhance their communication skills through immersive VR practice and AI-driven feedback.
                    </p>
                </div>

                <div style={featureGridStyle}>
                    <div style={featureBoxStyle}>
                        <h3 style={featureTitleStyle}>VR Practice Environment</h3>
                        <p style={featureTextStyle}>
                            Practice your presentations in realistic virtual environments to build confidence and master public speaking.
                        </p>
                    </div>

                    <div style={featureBoxStyle}>
                        <h3 style={featureTitleStyle}>AI Performance Analysis</h3>
                        <p style={featureTextStyle}>
                            Receive detailed feedback on your presentation style, pacing, and delivery through advanced AI analysis.
                        </p>
                    </div>

                    <div style={featureBoxStyle}>
                        <h3 style={featureTitleStyle}>Real-time Feedback</h3>
                        <p style={featureTextStyle}>
                            Get instant suggestions for improvement while practicing in our VR environment.
                        </p>
                    </div>
                </div>

                <div style={missionSectionStyle}>
                    <h2 style={missionTitleStyle}>Get Started</h2>
                    <p style={missionTextStyle}>
                        Ready to transform your presentation skills? Upload your slides and experience the future of presentation practice. For any questions, reach out to us at{' '}
                        <a href="mailto:awaaz.vr@gmail.com" style={contactLinkStyle}>
                            awaaz.vr@gmail.com
                        </a>
                    </p>
                </div>
            </div>
        </div>
    );
};



export default About;

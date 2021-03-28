import React from 'react';
import './App.css';

function Office365HealthStatus() {
  return (
    <React.Fragment>
      <header className="App-header">
         Current Microsoft Office 365 Health
      </header>
      <body>
        <HealthTable />
      </body> 
    </React.Fragment>
  )
}

class HealthTable extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      error: null,
      isLoaded: false,
      items: []
    };
  }

  // https://reactjs.org/docs/faq-ajax.html
    componentDidMount() {
      fetch("api/services") 
        .then(res => res.json())
        .then(
        (result) => {
                this.setState({
                    isLoaded: true,
                    items: result.value
            });
        },
        // Note: it's important to handle errors here
        // instead of a catch() block so that we don't swallow
        // exceptions from actual bugs in components.
        (error) => {
          this.setState({
            isLoaded: true,
            error
          });
        }
      )
  }

  render() {
    const { error, isLoaded, items } = this.state;
    if (error) {
      return <div>Error: {error.message}</div>;
    } else if (!isLoaded) {
      return <div>Loading...</div>;
    } else {
      return (
        <table className="main-table">
          {items.map(item => (
            <tr>
                  <td>{item.ServiceName}</td>
                  <td className={(item.Status === "ServiceOperational" ? "healthy" : "not-healthy")}>{item.Status}</td>
            </tr> 
          ))}
        </table>
      );
    }
  }
}

export default Office365HealthStatus;

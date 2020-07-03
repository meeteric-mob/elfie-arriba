import "./ErrorPage.scss";

// An error page to show in place of all content when access is denied or the site is down.
export default React.createClass({
    render: function () {

        if (this.props.status === 401){
            window.location.href = configuration.url + "/api/oauth";
        }

        return (
            <div className="errorPage">
                <h1>
                    <span className="errorTitle">Service Unavailable</span>
                    <span className="errorStatus">{this.props.status == 0 ? "" : this.props.status}</span>
                </h1>
                <article>
                    <p>
                        { this.props.status === 401
                            ? configuration.accessDeniedContent
                            : configuration.serviceUnavailableContent }
                    </p>
                </article>
            </div>
        );
    }
});

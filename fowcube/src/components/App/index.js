import React from 'react';
import { BrowserRouter as Router, Route } from 'react-router-dom';

import Navigation from '../Navigation';
import LandingPage from '../Landing';
import AccountPage from '../Account';
import AdminPage from '../Admin';
import SignInPage from '../SignIn';
import SignUpPage from '../SignUp';
import HomePage from '../Home';
import PasswordForgetPage from '../PasswordForget';

import * as ROUTES from '../../constants/routes';
import { AuthUserContext } from '../Session';
import { withAuthentication } from '../Session';

const App = () => (
	<AuthUserContext.Provider value={this.state.authUser}>
		<Router>
			<div>
				<Navigation />

				<hr />

				<Route exact path={ROUTES.LANDING} component={LandingPage} />
				<Route exact path={ROUTES.SIGN_UP} component={SignUpPage} />
				<Route exact path={ROUTES.ACCOUNT} component={AccountPage} />
				<Route exact path={ROUTES.ADMIN} component={AdminPage} />
				<Route exact path={ROUTES.SIGN_IN} component={SignInPage} />
				<Route exact path={ROUTES.HOME} component={HomePage} />
				<Route exact
					path={ROUTES.PASSWORD_FORGET}
					component={PasswordForgetPage} />
			</div>
		</Router>
	</AuthUserContext.Provider>
);

export default withAuthentication(App);
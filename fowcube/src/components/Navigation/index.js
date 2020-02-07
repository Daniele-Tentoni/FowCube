import React from 'react';
import { Link } from 'react-router-dom';
import { AuthUserContext } from '../Session';

import SignOutButton from '../SignOut';
import * as ROUTES from '../../constants/routes';

const Navigation = () => (
	<header className="mdc-top-app-bar mdc mdc-top-app-bar-fixed mdc-top-app-bar--dense">
		<div className="mdc-top-app-bar__row">
			<section className="mdc-top-app-bar__section mdc-top-app-bar__section--align-start">
				<button className="material-icons mdc-top-app-bar__navigation-icon mdc-icon-button">menu</button>
				<span className="mdc-top-app-bar__title">FowCube</span>
			</section>
			<section className="mdc-top-app-bar__section mdc-top-app-bar__section--align-end" role="toolbar">
				<Link to={ROUTES.LANDING}>Landing</Link>
				<button className="mdc-icon-button material-icons mdc-top-app-bar__action-item--unbounded" aria-label="Sign In">person</button>
				<button className="mdc-icon-button material-icons mdc-top-app-bar__action-item--unbounded" aria-label="Sign Up">person_add</button>
				<AuthUserContext.Consumer>
					{authUser =>
						authUser ? <NavigationAuth /> : <NavigationNonAuth />
					}
				</AuthUserContext.Consumer>
			</section>
		</div>
	</header>
);

const NavigationAuth = () => (
	<div>
		<Link to={ROUTES.HOME}>Home</Link>
		<Link to={ROUTES.ACCOUNT}>Account</Link>
		<Link to={ROUTES.ADMIN}>Admin</Link>
		<SignOutButton />
	</div>
);

const NavigationNonAuth = () => (
	<Link to={ROUTES.SIGN_IN}>Sign In</Link>
);

export default Navigation;
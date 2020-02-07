import React from 'react';
import { withFirebase } from '../Firebase';
const SignOutButton = ({ firebase }) => (
	<button className="mdc-icon-button material-icons mdc-top-app-bar__action-item--unbounded"
		aria-label="Sign Out" type="button" onClick={firebase.doSignOut}>
		indeterminate_check_box
	</button>
);
export default withFirebase(SignOutButton);
import React, { Component } from 'react';

/*
 * The createContext function creates two components. One, the
 * FirebaseContext.Provider, is used to provide a Firebase instance, and the
 * FirebaseContext.Consumer, that is used to retreice the Firebase instance
 * when needed.
 */
const FirebaseContext = React.createContext(null);

export const withFirebase = Component => props => (
	<FirebaseContext.Consumer>
		{firebase => <Component {...props} firebase={firebase} />}
	</FirebaseContext.Consumer>
)

export default FirebaseContext;
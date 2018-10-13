import React, { Component } from 'react';
import { Platform, StyleSheet, Text, View, TextInput, Button } from 'react-native';

export default class App extends Component {
  render() {
    const { container } = styles;
    return (
      <View style={container}>
        <TextInput />
        <Button />
        <Text></Text>
        <Text>{(Platform.OS).toUpperCase()}</Text>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F5FCFF',
  }
});
